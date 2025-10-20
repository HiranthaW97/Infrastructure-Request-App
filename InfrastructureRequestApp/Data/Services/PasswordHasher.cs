using System.Security.Cryptography;
using System.Text;

namespace InfrastructureRequestApp.Data.Services
{
	public class PasswordHasher
	{
		public static string Hash(string input)
		{
			using var sha = SHA256.Create();
			var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
			var sb = new StringBuilder();
			foreach (var b in bytes) sb.Append(b.ToString("X2"));
			return sb.ToString();
		}
	}
}
