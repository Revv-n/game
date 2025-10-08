namespace GreenT.HornyScapes;

public sealed class LastChanceFactory
{
	private readonly LastChanceManager _lastChanceManager;

	public LastChanceFactory(LastChanceManager lastChanceManager)
	{
		_lastChanceManager = lastChanceManager;
	}

	public void Create(int eventId, int calendarId, long calendarEndDate, long lastChanceDuration, LastChanceType lastChanceType)
	{
		long endDate = calendarEndDate + lastChanceDuration;
		LastChance entity = new LastChance(eventId, calendarId, calendarEndDate, endDate, lastChanceType);
		_lastChanceManager.Add(entity);
	}
}
