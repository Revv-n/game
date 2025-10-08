using System.Linq;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;

namespace GreenT.HornyScapes.Events;

public class SelloutStateService : BaseStateService<Sellout>
{
	private readonly SelloutManager _selloutSettingsProvider;

	public SelloutStateService(CalendarQueue calendarQueue, SelloutManager selloutSettingsProvider)
		: base(calendarQueue, EventStructureType.Sellout)
	{
		_selloutSettingsProvider = selloutSettingsProvider;
	}

	protected override Sellout GetModel(int eventId, int calendarId)
	{
		return _selloutSettingsProvider.Collection.FirstOrDefault((Sellout sellout) => sellout.ID == eventId);
	}
}
