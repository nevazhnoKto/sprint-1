namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Это унифицированный формат ответа, определённого стандартом RFC7807
	/// </summary>
	public class ProblemDetails
	{
		public string? Type { get; set; }

		public string? Title { get; set; }

		public int? Status { get; set; }

		public string? Detail { get; set; }

		public string? Instance { get; set; }

		public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.Ordinal);
	}
}
