namespace Domain.Enums
{
	/// <summary>
	/// Nекущий статус брони.
	/// </summary>
	public enum BookingStatus
	{
		/// <summary>
		/// В ожидании.
		/// </summary>
		Pending,

		/// <summary>
		/// Подтвержденный.
		/// </summary>
		Confirmed,

		/// <summary>
		/// Отклонен.
		/// </summary>
		Rejected,

		/// <summary>
		/// Отменен.
		/// </summary>
		Cancelled
	}
}