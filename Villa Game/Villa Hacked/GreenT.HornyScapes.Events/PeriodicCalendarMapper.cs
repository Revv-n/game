using System;
using GreenT.HornyScapes.Data;

namespace GreenT.HornyScapes.Events;

[Serializable]
[Mapper]
public class PeriodicCalendarMapper : CalendarMapper
{
	public long start_date;

	public int show_promo;
}
