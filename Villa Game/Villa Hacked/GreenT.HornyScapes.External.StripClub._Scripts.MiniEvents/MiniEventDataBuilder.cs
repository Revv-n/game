using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MiniEvents;
using StripClub.NewEvent.Save;

namespace GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;

public class MiniEventDataBuilder : ICalendarDataBuilder
{
	private MiniEventTimerController _miniEventTimerController;

	private readonly MiniEventMapperManager _mapperManager;

	public MiniEventDataBuilder(MiniEventTimerController miniEventTimerController, MiniEventMapperManager mapperManager)
	{
		_miniEventTimerController = miniEventTimerController;
		_mapperManager = mapperManager;
	}

	public void CreateData(CalendarModel calendarModel, string saveKey)
	{
		MiniEventMapper miniEventInfo = _mapperManager.GetMiniEventInfo(calendarModel.BalanceId);
		switch (miniEventInfo.config_type)
		{
		case global::GreenT.HornyScapes.MiniEvents.ConfigType.Activity:
			_miniEventTimerController.StartDefaultTimer(miniEventInfo.id, calendarModel.UniqID, calendarModel.RemainingTime);
			break;
		case global::GreenT.HornyScapes.MiniEvents.ConfigType.Rating:
			_miniEventTimerController.StartRatingTimer(miniEventInfo.id, calendarModel.UniqID, calendarModel.RemainingTime);
			break;
		}
	}
}
