# Endpoint Naming

This document records the endpoint naming migration for the FleetOps API.

## Conventions

- Use plural nouns for collections.
- Let the HTTP method describe CRUD actions.
- Use `camelCase` route parameters such as `{driverId}` and `{tripId}`.
- Use action subroutes only for state transitions or special operations.
- Use `me` when the resource is taken from the authenticated user.
- Keep role-specific resources under `/api/admin` and `/api/drivers`.

## Admin Drivers

| Before | After |
| --- | --- |
| `POST /api/admin/drivers/create` | `POST /api/admin/drivers` |
| `GET /api/admin/drivers/all` | `GET /api/admin/drivers` |
| `GET /api/admin/drivers/{driverId}` | Same |
| `PUT /api/admin/drivers/update/{driverId}` | `PUT /api/admin/drivers/{driverId}` |
| `DELETE /api/admin/drivers/delete/{driverId}` | `DELETE /api/admin/drivers/{driverId}` |
| `POST /api/admin/drivers/reset-password/{driverId}` | `POST /api/admin/drivers/{driverId}/password/reset` |

## Admin Fleet Managers

| Action | Route |
| --- | --- |
| Create | `POST /api/admin/fleet-managers` |
| List | `GET /api/admin/fleet-managers` |
| Detail | `GET /api/admin/fleet-managers/{userId}` |
| Update | `PUT /api/admin/fleet-managers/{userId}` |
| Delete | `DELETE /api/admin/fleet-managers/{userId}` |
| Reset password | `POST /api/admin/fleet-managers/{userId}/password/reset` |

## Admin Vehicles

| Before | After |
| --- | --- |
| `POST /api/admin/vehicles/create` | `POST /api/admin/vehicles` |
| `GET /api/admin/vehicles/all` | `GET /api/admin/vehicles` |
| `GET /api/admin/vehicles/{vehicleId}` | Same |
| `PUT /api/admin/vehicles/update/{vehicleId}` | `PUT /api/admin/vehicles/{vehicleId}` |
| `DELETE /api/admin/vehicles/delete/{vehicleId}` | `DELETE /api/admin/vehicles/{vehicleId}` |

## Profiles and Dashboards

| Before | After |
| --- | --- |
| `PUT /api/admin/profile/update` | `PUT /api/admin/profile/me` |
| `GET /api/admin/dashboard/{adminId}` | `GET /api/admin/dashboard/me` (supports `?bulan=&tahun=`) |
| - | `GET /api/admin/statistics` |
| `PUT /api/drivers/profile/select-vehicle` | `PUT /api/drivers/profile/me/vehicle` |

## Driver Fuel

| Before | After |
| --- | --- |
| `GET /api/drivers/fuel/history/me` | `GET /api/drivers/fuel/history` |
| `GET /api/drivers/fuel/detail/{fuelId}` | `GET /api/drivers/fuel/{fuelId}` |

## Driver Inspections

| Before | After |
| --- | --- |
| `POST /api/inspections/create` | `POST /api/drivers/inspections` |
| `GET /api/inspections/driver/{driverId}` | `GET /api/drivers/{driverId}/inspections` |
| `GET /api/inspections/vehicle/{vehicleId}` | `GET /api/drivers/vehicles/{vehicleId}/inspections` |
| `GET /api/inspections/detail/{id}` | `GET /api/drivers/inspections/{inspectionId}` |
| `PUT /api/inspections/update/{id}` | `PUT /api/drivers/inspections/{inspectionId}` |
| `DELETE /api/inspections/{id}` | `DELETE /api/drivers/inspections/{inspectionId}` |
| `GET /api/inspections/items` | `GET /api/drivers/inspection-items` |

## Driver Trips

| Before | After |
| --- | --- |
| `POST /api/trips/start` | `POST /api/drivers/trips` |
| `PATCH /api/trips/{id}/stop` | `PATCH /api/drivers/trips/{tripId}/stop` |
| `PATCH /api/trips/{id}/cancel` | `PATCH /api/drivers/trips/{tripId}/cancel` |
| `GET /api/trips/{id_driver}/trip` | `GET /api/drivers/{driverId}/trips` |
| `GET /api/trips/{id_trip}/details` | `GET /api/drivers/trips/{tripId}` |
| `GET /api/trips/driver/{driverId}/active` | `GET /api/drivers/{driverId}/trips/active` |

## Fleet Manager Maintenance

New endpoints. The HTTP method describes the action; only the status change is a sub-route because it is a state transition.

| Action | Route |
| --- | --- |
| Create schedule | `POST /api/fleet-manager/vehicles/{vehicleId}/maintenances` |
| List a vehicle's maintenances | `GET /api/fleet-manager/vehicles/{vehicleId}/maintenances` |
| Fleet maintenance summary | `GET /api/fleet-manager/vehicles/maintenance-status` |
| Detail | `GET /api/fleet-manager/maintenances/{maintenanceId}` |
| Edit schedule | `PUT /api/fleet-manager/maintenances/{maintenanceId}` |
| Delete schedule | `DELETE /api/fleet-manager/maintenances/{maintenanceId}` |
| Change status | `PUT /api/fleet-manager/maintenances/{maintenanceId}/status` |

## Driver Maintenance

| Action | Route |
| --- | --- |
| Maintenance status of the assigned vehicle | `GET /api/drivers/vehicles/me/maintenance-status` |

## Notifications

The same shape exists for both roles; only the prefix differs (`/api/fleet-manager` or `/api/drivers`).

| Action | Route |
| --- | --- |
| List notifications (`?unreadOnly=true`) | `GET {prefix}/notifications` |
| Mark one as read | `PATCH {prefix}/notifications/{notificationId}/read` |
| Mark all as read | `PATCH {prefix}/notifications/read-all` |

## Auth

These routes remain unchanged:

```text
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh
POST /api/auth/logout
POST /api/auth/logout-all
```

## Authorization

Admin endpoints should use `[Authorize(Roles = "Admin")]`.

Driver dashboard, profile, fuel, inspection, and trip endpoints should use `[Authorize(Roles = "Driver")]`.

Fleet Manager endpoints should use `[Authorize(Roles = "FleetManager")]` and read the organization from the `OrganizationId` claim, never from the request body. Driver notification and maintenance-status endpoints use `[Authorize(Roles = "Driver")]`.
