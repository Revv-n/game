using System;
using System.Linq;
using GreenT.HornyScapes.Events;
using Merge.Meta.RoomObjects;

namespace GreenT.HornyScapes.BattlePassSpace;

public class BattlePassStateService : BaseStateService<BattlePass>
{
	private new readonly CalendarQueue _calendarQueue;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	public BattlePassStateService(CalendarQueue calendarQueue, BattlePassSettingsProvider battlePassSettingsProvider)
		: base(calendarQueue, EventStructureType.BattlePass)
	{
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_calendarQueue = calendarQueue;
	}

	public IObservable<bool> OnBattlePassStateChange(EntityStatus state)
	{
		return _calendarQueue.OnCalendarStateChange(EventStructureType.BattlePass, state);
	}

	public bool HaveActiveBattlePass()
	{
		return _calendarQueue.HasActiveCalendar(EventStructureType.BattlePass);
	}

	public bool HaveBattlePassWithState(EntityStatus state)
	{
		return _calendarQueue.Any((CalendarModel calendar) => calendar.EventType == EventStructureType.BattlePass && calendar.CalendarState.Value == state);
	}

	protected override BattlePass GetModel(int eventId, int calendarId)
	{
		return _battlePassSettingsProvider.Collection.FirstOrDefault((BattlePass battlePass) => battlePass.ID == eventId);
	}
}
