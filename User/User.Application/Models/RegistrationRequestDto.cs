using User.Domain.Enums;

namespace User.Application.Models
{
	public class RegistrationRequestDto
	{
		public string? Login { get; set; }
		public string? Password { get; set; }
		public Roles Role { get; set; }
	}
}
