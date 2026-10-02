-- Query PostgreSQL untuk tabel maintenances (jadwal pemeliharaan kendaraan).
--
-- Tiap query berdiri sendiri: ubah nilai di CTE `params` (organization_id, ambang pengingat)
-- lalu jalankan di pgAdmin atau psql. Nilai default ambang sama dengan MaintenanceReminderOptions
-- (DaysBefore = 7, KmBefore = 500).
--
-- Status disimpan sebagai teks: 'Scheduled', 'InProgress', 'Completed', 'Cancelled'.
-- Jadwal jatuh tempo saat tanggal_jadwal ATAU target_odometer tercapai (lihat MaintenanceDueCalculator).
-- "Hari ini" mengikuti aplikasi, yaitu tanggal UTC.


-- 1. Semua jadwal pemeliharaan satu organisasi beserta status jatuh tempo.
--    Status jatuh tempo hanya dihitung untuk jadwal Scheduled.
WITH params AS (
    SELECT 1 AS organization_id, 7 AS days_before, 500 AS km_before
)
SELECT
    m.id,
    v.vehicle_name,
    v.plat_nomor,
    m.jenis,
    m.status,
    m.tanggal_jadwal,
    m.target_odometer,
    v.odometer AS odometer_saat_ini,
    m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date AS sisa_hari,
    m.target_odometer - v.odometer AS sisa_km,
    CASE
        WHEN m.status <> 'Scheduled' THEN NULL
        WHEN m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date < 0
            OR m.target_odometer - v.odometer <= 0 THEN 'Overdue'
        WHEN m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date <= p.days_before
            OR m.target_odometer - v.odometer <= p.km_before THEN 'DueSoon'
        ELSE 'Upcoming'
    END AS status_jatuh_tempo,
    m.created_at
FROM maintenances m
JOIN vehicles v ON v.id = m.vehicle_id
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
ORDER BY m.created_at DESC;


-- 2. Jadwal yang sudah mendekati jatuh tempo atau terlambat (Scheduled saja), terlambat lebih dulu.
WITH params AS (
    SELECT 1 AS organization_id, 7 AS days_before, 500 AS km_before
),
due AS (
    SELECT
        m.*,
        v.vehicle_name,
        v.plat_nomor,
        v.odometer AS odometer_saat_ini,
        m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date AS sisa_hari,
        m.target_odometer - v.odometer AS sisa_km,
        p.days_before,
        p.km_before
    FROM maintenances m
    JOIN vehicles v ON v.id = m.vehicle_id
    CROSS JOIN params p
    WHERE m.organization_id = p.organization_id
      AND m.status = 'Scheduled'
)
SELECT
    id,
    vehicle_name,
    plat_nomor,
    jenis,
    tanggal_jadwal,
    target_odometer,
    odometer_saat_ini,
    sisa_hari,
    sisa_km,
    CASE
        WHEN sisa_hari < 0 OR sisa_km <= 0 THEN 'Overdue'
        ELSE 'DueSoon'
    END AS status_jatuh_tempo
FROM due
WHERE sisa_hari < 0 OR sisa_km <= 0
   OR sisa_hari <= days_before OR sisa_km <= km_before
ORDER BY
    (sisa_hari < 0 OR sisa_km <= 0) DESC,
    tanggal_jadwal NULLS LAST,
    target_odometer NULLS LAST,
    id;


-- 3. Jadwal berikutnya per kendaraan: prioritas Overdue > DueSoon > Upcoming, lalu tanggal
--    terdekat, lalu target odometer terkecil (sama dengan MaintenanceDueCalculator.SelectNext).
WITH params AS (
    SELECT 1 AS organization_id, 7 AS days_before, 500 AS km_before
),
scheduled AS (
    SELECT
        m.id,
        m.vehicle_id,
        m.jenis,
        m.tanggal_jadwal,
        m.target_odometer,
        m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date AS sisa_hari,
        m.target_odometer - v.odometer AS sisa_km,
        CASE
            WHEN m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date < 0
                OR m.target_odometer - v.odometer <= 0 THEN 2
            WHEN m.tanggal_jadwal - (now() AT TIME ZONE 'UTC')::date <= p.days_before
                OR m.target_odometer - v.odometer <= p.km_before THEN 1
            ELSE 0
        END AS prioritas
    FROM maintenances m
    JOIN vehicles v ON v.id = m.vehicle_id
    CROSS JOIN params p
    WHERE m.organization_id = p.organization_id
      AND m.status = 'Scheduled'
)
SELECT DISTINCT ON (s.vehicle_id)
    v.id AS vehicle_id,
    v.vehicle_name,
    v.plat_nomor,
    s.id AS maintenance_id,
    s.jenis,
    s.tanggal_jadwal,
    s.target_odometer,
    s.sisa_hari,
    s.sisa_km,
    CASE s.prioritas WHEN 2 THEN 'Overdue' WHEN 1 THEN 'DueSoon' ELSE 'Upcoming' END AS status_jatuh_tempo
FROM scheduled s
JOIN vehicles v ON v.id = s.vehicle_id
ORDER BY
    s.vehicle_id,
    s.prioritas DESC,
    s.tanggal_jadwal NULLS LAST,
    s.target_odometer NULLS LAST,
    s.id;


-- 4. Ringkasan pemeliharaan seluruh armada: status kendaraan, jumlah jadwal terbuka
--    (Scheduled/InProgress), dan pemeliharaan selesai terakhir.
WITH params AS (
    SELECT 1 AS organization_id
),
last_completed AS (
    SELECT DISTINCT ON (m.vehicle_id)
        m.vehicle_id,
        m.jenis,
        m.tanggal,
        m.odometer,
        m.biaya
    FROM maintenances m
    CROSS JOIN params p
    WHERE m.organization_id = p.organization_id
      AND m.status = 'Completed'
    ORDER BY m.vehicle_id, m.tanggal DESC NULLS LAST, m.id DESC
)
SELECT
    v.id AS vehicle_id,
    v.vehicle_name,
    v.plat_nomor,
    v.status AS status_kendaraan,
    v.odometer AS odometer_saat_ini,
    COUNT(m.id) FILTER (WHERE m.status = 'Scheduled') AS jadwal_terjadwal,
    COUNT(m.id) FILTER (WHERE m.status = 'InProgress') AS sedang_dikerjakan,
    lc.jenis AS jenis_terakhir,
    lc.tanggal AS tanggal_terakhir,
    lc.odometer AS odometer_terakhir,
    lc.biaya AS biaya_terakhir
FROM vehicles v
CROSS JOIN params p
LEFT JOIN maintenances m
    ON m.vehicle_id = v.id
   AND m.organization_id = p.organization_id
   AND m.status IN ('Scheduled', 'InProgress')
LEFT JOIN last_completed lc ON lc.vehicle_id = v.id
WHERE v.organization_id = p.organization_id
GROUP BY v.id, v.vehicle_name, v.plat_nomor, v.status, v.odometer,
         lc.jenis, lc.tanggal, lc.odometer, lc.biaya
ORDER BY v.vehicle_name;


-- 5. Riwayat pemeliharaan selesai untuk satu kendaraan, terbaru lebih dulu.
WITH params AS (
    SELECT 1 AS organization_id, 1 AS vehicle_id
)
SELECT
    m.id,
    m.jenis,
    m.tanggal,
    m.odometer,
    m.biaya,
    m.catatan
FROM maintenances m
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
  AND m.vehicle_id = p.vehicle_id
  AND m.status = 'Completed'
ORDER BY m.tanggal DESC NULLS LAST, m.id DESC;


-- 6. Total biaya pemeliharaan selesai per kendaraan dalam rentang tanggal.
WITH params AS (
    SELECT 1 AS organization_id,
           TIMESTAMPTZ '2026-01-01 00:00:00+00' AS dari,
           TIMESTAMPTZ '2027-01-01 00:00:00+00' AS sampai   -- batas atas eksklusif
)
SELECT
    v.id AS vehicle_id,
    v.vehicle_name,
    v.plat_nomor,
    COUNT(m.id) AS jumlah_pemeliharaan,
    COALESCE(SUM(m.biaya), 0) AS total_biaya,
    ROUND(AVG(m.biaya), 2) AS rata_rata_biaya
FROM maintenances m
JOIN vehicles v ON v.id = m.vehicle_id
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
  AND m.status = 'Completed'
  AND m.tanggal >= p.dari
  AND m.tanggal < p.sampai
GROUP BY v.id, v.vehicle_name, v.plat_nomor
ORDER BY total_biaya DESC;


-- 7. Biaya pemeliharaan selesai per bulan untuk seluruh organisasi.
WITH params AS (
    SELECT 1 AS organization_id
)
SELECT
    date_trunc('month', m.tanggal) AS bulan,
    COUNT(*) AS jumlah_pemeliharaan,
    COALESCE(SUM(m.biaya), 0) AS total_biaya
FROM maintenances m
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
  AND m.status = 'Completed'
GROUP BY date_trunc('month', m.tanggal)
ORDER BY bulan DESC;


-- 8. Jumlah pemeliharaan per status dan jenis.
WITH params AS (
    SELECT 1 AS organization_id
)
SELECT
    m.status,
    m.jenis,
    COUNT(*) AS jumlah
FROM maintenances m
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
GROUP BY m.status, m.jenis
ORDER BY m.status, jumlah DESC, m.jenis;


-- 9. Jadwal Scheduled yang notifikasinya belum terkirim (kandidat pemindaian pengingat).
--    reminder_sent_at = pengingat jatuh tempo, overdue_notified_at = pemberitahuan terlambat.
WITH params AS (
    SELECT 1 AS organization_id
)
SELECT
    m.id,
    v.vehicle_name,
    m.jenis,
    m.tanggal_jadwal,
    m.target_odometer,
    m.reminder_sent_at,
    m.overdue_notified_at
FROM maintenances m
JOIN vehicles v ON v.id = m.vehicle_id
CROSS JOIN params p
WHERE m.organization_id = p.organization_id
  AND m.status = 'Scheduled'
  AND (m.reminder_sent_at IS NULL OR m.overdue_notified_at IS NULL)
ORDER BY m.tanggal_jadwal NULLS LAST, m.id;


-- 10. Pemeriksaan konsistensi status kendaraan dan pemeliharaan.
--     Kendaraan berstatus 'Maintenance' harus punya pemeliharaan InProgress, dan sebaliknya.
WITH params AS (
    SELECT 1 AS organization_id
)
SELECT
    v.id AS vehicle_id,
    v.vehicle_name,
    v.status AS status_kendaraan,
    COUNT(m.id) FILTER (WHERE m.status = 'InProgress') AS pemeliharaan_inprogress,
    CASE
        WHEN v.status = 'Maintenance' AND COUNT(m.id) FILTER (WHERE m.status = 'InProgress') = 0
            THEN 'Kendaraan Maintenance tanpa pemeliharaan InProgress'
        WHEN v.status <> 'Maintenance' AND COUNT(m.id) FILTER (WHERE m.status = 'InProgress') > 0
            THEN 'Ada pemeliharaan InProgress tapi kendaraan bukan Maintenance'
    END AS masalah
FROM vehicles v
CROSS JOIN params p
LEFT JOIN maintenances m ON m.vehicle_id = v.id
WHERE v.organization_id = p.organization_id
GROUP BY v.id, v.vehicle_name, v.status
HAVING (v.status = 'Maintenance' AND COUNT(m.id) FILTER (WHERE m.status = 'InProgress') = 0)
    OR (v.status <> 'Maintenance' AND COUNT(m.id) FILTER (WHERE m.status = 'InProgress') > 0)
ORDER BY v.vehicle_name;
