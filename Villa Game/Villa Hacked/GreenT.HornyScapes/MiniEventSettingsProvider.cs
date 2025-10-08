using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.Model.Collections;
using GreenT.Types;

namespace GreenT.HornyScapes;

public sealed class MiniEventSettingsProvider : SimpleManager<MiniEvent>
{
	public MiniEvent GetEvent(int eventId)
	{
		return Collection.FirstOrDefault((MiniEvent x) => x.EventId == eventId);
	}

	public MiniEvent GetMiniEventByCurrencyId(CompositeIdentificator id)
	{
		return Collection.FirstOrDefault((MiniEvent x) => x.CurrencyIdentificator == id);
	}
}
