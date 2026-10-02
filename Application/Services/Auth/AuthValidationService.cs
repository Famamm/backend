using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace fleetops_backend.Application.Services
{
	public class AuthValidationService : IAuthValidationService
	{
		private static readonly EmailAddressAttribute EmailValidator = new();
		private static readonly Regex PhoneNumberRegex = new(
			@"^\+?[0-9\s\-().]+$",
			RegexOptions.Compiled);
		public bool IsValidPassword(string? password)
		{
			return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
		}
		public bool IsValidEmail(string? email)
		{
			return !string.IsNullOrWhiteSpace(email) && EmailValidator.IsValid(email);
		}
		public bool IsValidFullName(string? fullName)
		{
			return !string.IsNullOrWhiteSpace(fullName) &&
				fullName.Trim().Length >= 2;
		}
		public bool IsValidPhoneNumber(string? phoneNumber)
		{
			if (string.IsNullOrWhiteSpace(phoneNumber))
				return false;

			var trimmed = phoneNumber.Trim();
			return trimmed.Length >= 8 &&
				trimmed.Length <= 20 &&
				PhoneNumberRegex.IsMatch(trimmed);
		}
		public bool IsValidOrganizationName(string? organizationName)
		{
			return !string.IsNullOrWhiteSpace(organizationName) &&
				organizationName.Trim().Length >= 2;
		}

		public bool IsValidIdentity(string? identity)
		{
			return !string.IsNullOrWhiteSpace(identity);
		}

		public bool IsValidRefreshToken(string? refreshToken)
		{
			return !string.IsNullOrWhiteSpace(refreshToken);
		}
	}
}
