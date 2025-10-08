using GreenT.Types;
using StripClub.Model;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class CurrencyFlyPool : AbstractPool<MiniEventFlyingCurrencyView>
{
	public MiniEventFlyingCurrencyView GetInstance(CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
	{
		MiniEventFlyingCurrencyView instance = GetInstance();
		instance.Setup(currencyType, currencyIdentificator);
		return instance;
	}
}
