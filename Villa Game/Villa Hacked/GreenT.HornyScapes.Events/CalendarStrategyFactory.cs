using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.MiniEvents;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class CalendarStrategyFactory : IFactory<EventStructureType, CalendarModel, ICalendarStateStrategy>, IFactory
{
	private readonly DiContainer container;

	private readonly IContentAdder contentAdder;

	private CalendarStrategyFactory(DiContainer container, IContentAdder contentAdder)
	{
		this.container = container;
		this.contentAdder = contentAdder;
	}

	public ICalendarStateStrategy Create(EventStructureType structureType, CalendarModel calendarModel)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		return structureType switch
		{
			EventStructureType.Event => container.Instantiate<EventCalendarStateStrategy>((IEnumerable<object>)new object[2] { calendarModel, contentAdder }), 
			EventStructureType.BattlePass => container.Instantiate<BattlePassCalendarStateStrategy>((IEnumerable<object>)new object[1] { calendarModel }), 
			EventStructureType.Mini => container.Instantiate<MiniEventCalendarStateStrategy>((IEnumerable<object>)new object[1] { calendarModel }), 
			EventStructureType.Sellout => container.Instantiate<SelloutCalendarStateStrategy>((IEnumerable<object>)new object[1] { calendarModel }), 
			_ => throw new SwitchExpressionException((object)structureType), 
		};
	}
}
