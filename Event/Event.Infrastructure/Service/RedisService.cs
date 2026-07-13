using Event.Application.Interfaces;
using Event.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Event.Infrastructure.Service
{
#pragma warning disable CS1591

	public class RedisService : IRedisService
	{
		private readonly IDatabase _redis;
		private readonly ILogger<RedisService> _logger;

		public RedisService(IConnectionMultiplexer multiplexer, ILogger<RedisService> logger)
		{
			_redis = multiplexer.GetDatabase();
			_logger = logger;
		}

		public async Task<bool> DeleteCacheAsync(Guid eventId)
		{
			try
			{
				var cacheKey = GetCacheKeyByEventId(eventId);
				return await _redis.KeyDeleteAsync(cacheKey);
			}
			catch(Exception ex)
			{
				_logger.LogError($"При удалении Кэша для eventId = {eventId} произошла ошибка. {ex.Message}");
			}
			return false;
			
		}

		public async Task<EventModel?> GetCacheForIdAsync(Guid eventId)
		{
			try
			{
				var cacheKey = GetCacheKeyByEventId(eventId);

				var cached = await _redis.StringGetAsync(cacheKey);
				if (cached.HasValue && !cached.IsNullOrEmpty)
					return JsonSerializer.Deserialize<EventModel>(cached!);
			}
			catch(Exception ex)
			{
				_logger.LogError($"При получении Кэша для eventId = {eventId} произошла ошибка. {ex.Message}");
			}
			return null;
		}

		public async Task<bool> SetCacheAsync(Guid eventId, EventModel value)
		{
			if (value == null)
				return false;
			try
			{
				var cacheKey = GetCacheKeyByEventId(eventId);
				return await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(value), TimeSpan.FromMinutes(5));
			}
			catch(Exception ex)
			{
				_logger.LogError($"При записи Кэша для eventId = {eventId} произошла ошибка. {ex.Message}");
			}
			return false;
		}

		public async Task<List<EventModel>?> GetTop10EventsAsync()
		{
			try
			{
				var key = GetCacheKeyByTop10();
				var cached = await _redis.StringGetAsync(key);
				if (cached.HasValue && !cached.IsNullOrEmpty)
				{
					return JsonSerializer.Deserialize<List<EventModel>>(cached!);
				}
			}
			catch(Exception ex)
			{
				_logger.LogError($"При получении Кэша для 10 событий с наибольшим процентом проданных мест произошла ошибка. {ex.Message}");
			}
			return null;
		}

		public async Task<bool> SetTop10EventsAsync(List<EventModel?> topEvents)
		{
			if (topEvents == null)
				return false;
			try
			{
				var key = GetCacheKeyByTop10();
				return await _redis.StringSetAsync(key, JsonSerializer.Serialize(topEvents), TimeSpan.FromMinutes(15));
			}
			catch (Exception ex)
			{
				_logger.LogError($"При записи Кэша для 10 событий с наибольшим процентом проданных мест произошла ошибка. {ex.Message}");
			}
			return false;

		}

		private string GetCacheKeyByEventId(Guid evenId)
		{
			return $"event:{evenId}";
		}
		private string GetCacheKeyByTop10()
		{
			return $"events:top10";
		}
	}
#pragma warning restore CS1591

}
