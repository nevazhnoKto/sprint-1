using Microsoft.OpenApi.Writers;
using WebApiTamakulov.Interfaces;

namespace WebApiTamakulov.Services.BackgroundServices
{
	/// <summary>
	/// Фоновая обработка бронирования.
	/// </summary>
	public class ConfirmBookingBackgroundService : BackgroundService
	{
		private readonly ILogger<BackgroundService> _logger;
		private readonly IServiceScopeFactory _serviceScope;

		/// <summary>
		/// Фоновая обработка бронирования.
		/// </summary>
		public ConfirmBookingBackgroundService(IServiceScopeFactory serviceScope, ILogger<BackgroundService> logger)
		{
			_serviceScope = serviceScope;
			_logger = logger;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stoppingToken">Токен отмены.</param>
		/// <returns></returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("ConfirmBookingBackgroundService запущен.");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _serviceScope.CreateScope();
					var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
					var bookings = bookingService.GetAllPendingStatusBookingAsync();
					foreach (var booking in bookings)
					{
						await Task.Delay(2000);
						bookingService.UpdateStatusBookingAsync(booking.Id, Enums.BookingStatus.Confirmed);
					}
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Ошибка при подтвержнеии бронирования.");
				}
				// Запускать каждые 5 сек.
				await Task.Delay(5000);
			}

			_logger.LogInformation("ConfirmBookingBackgroundService остановлен");
		}
	}
}
