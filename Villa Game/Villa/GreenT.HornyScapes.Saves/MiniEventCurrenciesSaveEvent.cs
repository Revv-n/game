using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class MiniEventCurrenciesSaveEvent : SaveEvent
{
	private const CurrencyType _currencyType = CurrencyType.MiniEvent;

	private ICurrencyProcessor _currencyProcessor;

	private Currencies _mainBalance;

	[Inject]
	public void Init(Currencies mainBalance, ICurrencyProcessor currencyProcessor)
	{
		_mainBalance = mainBalance;
		_currencyProcessor = currencyProcessor;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		_mainBalance.OnNewCurrency.Where(((CurrencyType, SimpleCurrency) tuple) => tuple.Item1 == CurrencyType.MiniEvent).SelectMany(((CurrencyType, SimpleCurrency) tuple) => _currencyProcessor.GetCountReactiveProperty(CurrencyType.MiniEvent, tuple.Item2.Identificator)).Skip(1)
			.Subscribe(delegate
			{
				Save();
			})
			.AddTo(this);
	}
}
