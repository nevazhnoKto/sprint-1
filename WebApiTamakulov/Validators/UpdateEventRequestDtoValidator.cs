using FluentValidation;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Validators
{
	/// <summary>
	/// Валидация EventDto.
	/// </summary>
	public class UpdateEventRequestDtoValidator : AbstractValidator<UpdateEventRequestDto>
	{
		/// <summary>
		/// Валидация UpdateEventRequestDto.
		/// </summary>
		public UpdateEventRequestDtoValidator()
		{
			RuleFor(x => x.Id)
			.NotEmpty().WithMessage("ID обязателен");

			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Значение Title обязательно для заполнения");

			RuleFor(x => x.TotalSeats)
				 .NotEmpty().WithMessage("Значение TotalSeats обязательно для заполнения")
				 .GreaterThan(0).WithMessage("TotalSeats должно быть больше нуля");

			RuleFor(x => x.AvailableSeats)
				 .NotEmpty().WithMessage("Значение TotalSeats обязательно для заполнения")
				 .GreaterThanOrEqualTo(0).WithMessage("AvailableSeats должно быть больше нуля");

			RuleFor(x => x.StartAt)
				.NotEmpty().WithMessage("Значение StartAt обязательно для заполнения");

			RuleFor(x => x.EndAt)
				.NotEmpty().WithMessage("Значение EndAt обязательно для заполнения")
				.GreaterThan(x => x.StartAt).WithMessage("Дата окончания должна быть позже даты начала");
		}
	}
}
