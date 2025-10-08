using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class BattlePassLevelContentFactory : IFactory<int, LinkedContentAnalyticData, LinkedContent, BattlePassLevelLinkedContent>, IFactory, IFactory<int, LinkedContentAnalyticData, BattlePassLevelLinkedContent>
{
	private readonly ICurrencyProcessor currencyProcessor;

	private readonly CurrencyAnalyticProcessingService battlePassAnalytic;

	private readonly BattlePassProvider _provider;

	private readonly BattlePassStateService _stateService;

	private readonly GameSettings gameSettings;

	private readonly BattlePassSettingsProvider _settingsProvider;

	private readonly BattlePassMapperProvider _mapperProvider;

	public BattlePassLevelContentFactory(CurrencyAnalyticProcessingService battlePassAnalytic, ICurrencyProcessor currencyProcessor, BattlePassProvider provider, GameSettings gameSettings, BattlePassStateService stateService, BattlePassSettingsProvider settingsProvider, BattlePassMapperProvider mapperProvider)
	{
		_provider = provider;
		_stateService = stateService;
		this.battlePassAnalytic = battlePassAnalytic;
		this.currencyProcessor = currencyProcessor;
		this.gameSettings = gameSettings;
		_settingsProvider = settingsProvider;
		_mapperProvider = mapperProvider;
	}

	public BattlePassLevelLinkedContent Create(int quantity, LinkedContentAnalyticData analyticData, LinkedContent next = null)
	{
		return new BattlePassLevelLinkedContent(gameSettings, quantity, currencyProcessor, battlePassAnalytic, analyticData, _stateService, _provider, _settingsProvider, _mapperProvider, next);
	}

	public BattlePassLevelLinkedContent Create(int quantity, LinkedContentAnalyticData analyticData)
	{
		return Create(quantity, analyticData, null);
	}
}
