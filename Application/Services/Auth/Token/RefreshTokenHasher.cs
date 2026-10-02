using System.Security.Cryptography;
using System.Text;

namespace fleetops_backend.Application.Services
{
	public static class RefreshTokenHasher
	{
		public static string Hash(string refreshToken)
		{
			var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
			return Convert.ToHexString(hash);
		}
	}
}
