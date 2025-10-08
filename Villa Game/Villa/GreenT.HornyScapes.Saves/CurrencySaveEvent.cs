using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class CurrencySaveEvent : SaveEvent
{
	[SerializeField]
	private CurrencyType currency;

	private ICurrencyProcessor currencyProcessor;

	private GameStarter gameStarter;

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, GameStarter gameStarter)
	{
		this.currencyProcessor = currencyProcessor;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		gameStarter.IsGameActive.FirstOrDefault((bool x) => x).ContinueWith((bool _) => currencyProcessor.GetCountReactiveProperty(currency)).Skip(1)
			.Subscribe(delegate
			{
				Save();
			})
			.AddTo(this);
	}
}
