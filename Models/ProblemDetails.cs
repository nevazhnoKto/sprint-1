namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Это унифицированный формат ответа, определённого стандартом RFC7807
	/// </summary>
	public class ProblemDetails
	{
		/// <summary>
		/// URI ссылка, которая идентифицирует тип возникшей проблемы.
		/// </summary>
		public string? Type { get; set; }

		/// <summary>
		/// Краткое, понятное человеку описание типа проблемы.
		/// </summary>
		public string? Title { get; set; }

		/// <summary>
		/// HTTP-статус код, сгенерированный сервером для данного ответа.
		/// </summary>
		public int? Status { get; set; }

		/// <summary>
		/// Детальное, специфичное для данного экземпляра проблемы, описание.
		/// </summary>
		public string? Detail { get; set; }

		/// <summary>
		/// URI ссылка, указывающая на конкретный экземпляр проблемы.
		/// </summary>
		public string? Instance { get; set; }

		/// <summary>
		/// Расширения.
		/// </summary>
		public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.Ordinal);
	}
}
