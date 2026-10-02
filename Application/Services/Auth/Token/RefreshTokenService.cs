using Microsoft.EntityFrameworkCore;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public RefreshTokenService(
            AppDbContext context,
            ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        private RefreshToken CreateRefreshToken(int userId, string rawToken)
        {
            var now = DateTimeOffset.UtcNow;

            return new RefreshToken
            {
                UserId = userId,
                Token = RefreshTokenHasher.Hash(rawToken),
                CreatedAt = now,
                ExpiresAt = now.AddDays(_tokenService.GetRefreshTokenExpiryDays())
            };
        }

        public async Task<string> CreateAsync(int userId)
        {
            var rawToken = _tokenService.GenerateRefreshToken();
            var refreshToken = CreateRefreshToken(userId, rawToken);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return rawToken;
        }

        public async Task<RefreshToken?> FindValidAsync(string rawToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken))
                return null;

            var hash = RefreshTokenHasher.Hash(rawToken);

            // RevokedAt dipakai di query (kolom asli), bukan IsRevoked —
            // IsRevoked itu computed property tanpa kolom di DB, jadi
            // EF Core tidak bisa menerjemahkannya ke SQL.
            var token = await _context.RefreshTokens
                .Include(token => token.User)
                .ThenInclude(user => user!.Role)
                .FirstOrDefaultAsync(token =>
                    token.Token == hash &&
                    token.RevokedAt == null &&
                    token.ExpiresAt > DateTimeOffset.UtcNow);

            return token;
        }

        public async Task<(RefreshToken Entity, string RawToken)> RotateAsync(
            RefreshToken currentToken,
            int userId)
        {
            currentToken.RevokedAt = DateTimeOffset.UtcNow;

            var rawToken = _tokenService.GenerateRefreshToken();
            var replacement = CreateRefreshToken(userId, rawToken);

            _context.RefreshTokens.Update(currentToken);
            _context.RefreshTokens.Add(replacement);

            await _context.SaveChangesAsync();

            return (replacement, rawToken);
        }

        public async Task<bool> RevokeAsync(string rawToken)
        {
            var token = await FindValidAsync(rawToken);

            if (token == null)
                return false;

            token.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RevokeAllAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
                token.RevokedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}