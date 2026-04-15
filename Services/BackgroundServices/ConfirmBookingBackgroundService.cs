using Microsoft.OpenApi.Writers;
using System.Linq.Expressions;
using System.Reflection;
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
		private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

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
					var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
					var pendingBookings = bookingService.GetAllPendingStatusBookingAsync();
					var tasks = pendingBookings.Select(booking => ProcessBookingAsync(bookingService, eventService, booking, stoppingToken));
					await Task.WhenAll(tasks);
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

		private async Task ProcessBookingAsync(IBookingService bookingService, IEventService eventService, Booking booking, CancellationToken stoppingToken)
		{
			await Task.Delay(2000, stoppingToken);
			await _processingSemaphore.WaitAsync(stoppingToken);
			var eventInfo = eventService.GetById(booking.EventId);
			try
			{
				if (eventInfo != null)
					bookingService.ConfirmBookingAsync(booking.Id);
				else
				{
					bookingService.RejectedBookingAsync(booking.Id);
					_logger.LogWarning($"Событие №{booking.EventId} удалено , бронь отклонена");
				}
			}
			catch
			{
				bookingService.RejectedBookingAsync(booking.Id, booking.EventId);
			}
			finally
			{
				_processingSemaphore.Release();
			}
			
			
		}
	}
}
