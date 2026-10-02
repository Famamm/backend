using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace fleetops_backend.Common
{
    public static class PasswordGenerator
    {
        private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ"; // removed ambiguous I,O
        private const string Lower = "abcdefghijkmnopqrstuvwxyz"; // removed ambiguous l
        private const string Digits = "23456789"; // removed 0,1
        private const string Symbols = "!@#$%^&*()-_=+[]{}<>?";

        public static string Generate(int length = 12)
        {
            if (length < 8)
                throw new ArgumentException("Password length must be at least 8", nameof(length));

            var rng = RandomNumberGenerator.Create();
            var required = new List<char>
            {
                GetRandomChar(Upper, rng),
                GetRandomChar(Lower, rng),
                GetRandomChar(Digits, rng),
                GetRandomChar(Symbols, rng)
            };

            var allChars = Upper + Lower + Digits + Symbols;
            var result = new List<char>(required);

            for (int i = required.Count; i < length; i++)
            {
                result.Add(GetRandomChar(allChars, rng));
            }

            // Shuffle
            var shuffled = result.ToArray();
            Shuffle(shuffled, rng);

            return new string(shuffled);
        }

        public static string GenerateForUserOrg(string fullName, string organizationName, int? seed = null)
        {
            string Normalize(string v)
            {
                if (string.IsNullOrWhiteSpace(v)) return "user";
                var normalized = v.Trim().ToLowerInvariant();
                var sb = new StringBuilder();
                foreach (var ch in normalized)
                {
                    if ((ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9')) sb.Append(ch);
                }
                return sb.Length == 0 ? "user" : sb.ToString();
            }

            var namePart = Normalize(fullName);
            var orgPart = Normalize(organizationName);
            var basePart = $"{namePart}@{orgPart}";

            int number;
            if (seed.HasValue)
            {
                var rng = new Random(seed.Value);
                number = rng.Next(0, 10000);
            }
            else
            {
                var buf = new byte[4];
                RandomNumberGenerator.Create().GetBytes(buf);
                number = (int)(BitConverter.ToUInt32(buf, 0) % 10000u);
            }

            var suffix = number.ToString("D4");
            var result = $"{basePart}/{suffix}";

            // Ensure minimum length of 8
            if (result.Length < 8)
            {
                var extra = new byte[2];
                RandomNumberGenerator.Create().GetBytes(extra);
                var extraNum = BitConverter.ToUInt16(extra, 0) % 1000;
                result = result + extraNum.ToString("D3");
            }

            return result;
        }

        private static char GetRandomChar(string chars, RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            while (true)
            {
                rng.GetBytes(buffer);
                uint value = BitConverter.ToUInt32(buffer, 0);
                var idx = (int)(value % (uint)chars.Length);
                return chars[idx];
            }
        }

        private static void Shuffle(char[] array, RandomNumberGenerator rng)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                var buffer = new byte[4];
                rng.GetBytes(buffer);
                uint value = BitConverter.ToUInt32(buffer, 0);
                int j = (int)(value % (uint)(i + 1));
                var tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }
    }
}
