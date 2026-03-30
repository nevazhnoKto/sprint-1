using FluentValidation;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Validators
{
	/// <summary>
	/// Валидация EventDto.
	/// </summary>
	public class GetEventsRequestValidator : AbstractValidator<GetEventsRequest>
	{
		/// <summary>
		/// Валидация EventDto.
		/// </summary>
		public GetEventsRequestValidator()
		{
			RuleFor(x => x.Page)
			.GreaterThan(0)
			.WithMessage("Номер страницы должен быть больше 0");

			RuleFor(x => x.PageSize)
			.GreaterThan(0)
			.WithMessage("Размер страницы должен быть больше 0");

			// Валидация диапазона дат
			RuleFor(x => x)
				.Must(BeValidDateRange)
				.WithMessage("Дата 'from' должна быть раньше или равна дате 'to'")
				.When(x => x.From.HasValue && x.To.HasValue);

		}

		private bool BeValidDateRange(GetEventsRequest request)
		{
			return request.From!.Value <= request.To!.Value;
		}
	}
}
