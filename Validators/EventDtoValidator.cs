using FluentValidation;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Validators
{
	public class EventDtoValidator : AbstractValidator<EventDto>
	{
		public EventDtoValidator()
		{
			RuleFor(x => x.Id)
			.NotEmpty().WithMessage("ID обязателен")
			.GreaterThan(0).WithMessage("ID должен быть положительным числом");

			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Значение Title обязательно для заполнения");

			RuleFor(x => x.StartAt)
				.NotEmpty().WithMessage("Значение StartAt обязательно для заполнения");

			RuleFor(x => x.EndAt)
				.NotEmpty().WithMessage("Значение EndAt обязательно для заполнения")
				.GreaterThan(x => x.StartAt).WithMessage("Дата окончания должна быть позже даты начала");
		}
	}
}
