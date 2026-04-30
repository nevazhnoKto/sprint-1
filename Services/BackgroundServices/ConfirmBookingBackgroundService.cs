using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Enums;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

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
					List<Booking> pendingBookings;
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

		private async Task ProcessBookingAsync(Booking booking, CancellationToken stoppingToken)
		{

			using (var scope = _serviceScope.CreateScope())
			{
				var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
				var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
				var eventInfo = eventService.GetById(booking.EventId);
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
			}
		}
	}
}
