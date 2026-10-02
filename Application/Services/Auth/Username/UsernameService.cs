
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using fleetops_backend.Infrastructure.Data;

namespace fleetops_backend.Application.Services
{
    public class UsernameService : IUsernameService
    {
        private readonly AppDbContext _context;

        public UsernameService(AppDbContext context)
        {
            _context = context;
        }

        private static string NormalizeUsernamePart(string value)
        {
            var normalized = value.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9]+", "");
            
            return string.IsNullOrEmpty(normalized) ? "user" : normalized;
        }

        public async Task<string> GenerateUniqueUsernameAsync(string fullName, string organizationName)
        {
            var baseUsername = $"{NormalizeUsernamePart(fullName)}@{NormalizeUsernamePart(organizationName)}";
            var username = baseUsername;
            var suffix = 2;

            while (await _context.Users.AnyAsync(user => user.Username == username))
            {
                username = $"{baseUsername}{suffix}";
                suffix++;
            }

            return username;
        }
    }
}