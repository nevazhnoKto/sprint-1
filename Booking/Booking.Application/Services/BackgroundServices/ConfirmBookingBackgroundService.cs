using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Booking.Domain.Enums;
using Booking.Application.Interfaces;
using Booking.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Services.BackgroundServices
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
					List<BookingModel> pendingBookings;
					using (var scope = _serviceScope.CreateScope())
					{
						var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
						pendingBookings = await bookingService.GetAllPendingStatusBookingAsync();
					}

					// Для каждого бронирования - свой scope
					foreach (var booking in pendingBookings)
					{
						await ProcessBookingAsync(booking, stoppingToken);
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
				await Task.Delay(5000, stoppingToken);
			}

			_logger.LogInformation("ConfirmBookingBackgroundService остановлен");
		}

		private async Task ProcessBookingAsync(BookingModel booking, CancellationToken stoppingToken)
		{

			/*using (var scope = _serviceScope.CreateScope())
			{
				var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
				var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
				var eventInfo = await eventService.GetById(booking.EventId);
				try
				{
					if (eventInfo != null)
						await bookingService.ConfirmBookingAsync(booking.Id);
					else
					{
						await bookingService.RejectedBookingAsync(booking.Id);
						_logger.LogWarning($"Событие №{booking.EventId} удалено , бронь отклонена");
					}
				}
				catch
				{
					await bookingService.RejectedBookingAsync(booking.Id, booking.EventId);
				}
			}*/
		}
	}
}
