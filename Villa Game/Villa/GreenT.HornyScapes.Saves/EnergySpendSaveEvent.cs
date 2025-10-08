using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class EnergySpendSaveEvent : SaveEvent
{
	[SerializeField]
	private CurrencyType currencyType;

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
		(from _diff in (from _energy in gameStarter.IsGameActive.First((bool _isActive) => _isActive).ContinueWith((bool _) => currencyProcessor.GetCountReactiveProperty(currencyType)).Pairwise()
				select _energy.Previous - _energy.Current into _diff
				where _diff > 0
				select _diff).Scan((int _previous, int _current) => _previous + _current)
			where _diff % 10 == 0
			select _diff).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}
