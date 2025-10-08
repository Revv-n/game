using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Sellouts.Providers;
using StripClub.NewEvent.Save;

namespace GreenT.HornyScapes.Sellouts.Services;

public class SelloutDataBuilder : ICalendarDataBuilder
{
	private readonly SelloutManager _selloutManager;

	public SelloutDataBuilder(SelloutManager selloutManager)
	{
		_selloutManager = selloutManager;
	}

	public void CreateData(CalendarModel calendarModel, string saveKey)
	{
		_selloutManager.TryGetSellout(calendarModel.BalanceId, out var _);
	}
}
