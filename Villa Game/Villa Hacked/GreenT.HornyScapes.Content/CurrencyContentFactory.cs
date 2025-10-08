using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class CurrencyContentFactory : IFactory<int, CurrencyType, LinkedContentAnalyticData, CompositeIdentificator, CurrencyLinkedContent>, IFactory, IFactory<int, CurrencyType, LinkedContentAnalyticData, CompositeIdentificator, LinkedContent, CurrencyLinkedContent>
{
	private readonly GameSettings gameSettings;

	private readonly IPlayerExpController playerExpController;

	private readonly IPlayerBasics playerBasics;

	private readonly CurrencyAnalyticProcessingService currencyAmplitudeAnalytic;

	private readonly ICurrencyProcessor currencyProcessor;

	public CurrencyContentFactory(GameSettings gameSettings, IPlayerExpController playerExpController, IPlayerBasics playerBasics, CurrencyAnalyticProcessingService currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor)
	{
		this.gameSettings = gameSettings;
		this.playerExpController = playerExpController;
		this.playerBasics = playerBasics;
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.currencyProcessor = currencyProcessor;
	}

	public CurrencyLinkedContent Create(int quantity, CurrencyType type, LinkedContentAnalyticData analyticData, CompositeIdentificator compositeIdentificator, LinkedContent nestedContent = null)
	{
		return new CurrencyLinkedContent(quantity, type, analyticData, gameSettings, playerExpController, playerBasics, currencyAmplitudeAnalytic, currencyProcessor, compositeIdentificator, nestedContent);
	}

	public CurrencyLinkedContent Create(int quantity, CurrencyType type, LinkedContentAnalyticData analyticData, CompositeIdentificator compositeIdentificator)
	{
		return Create(quantity, type, analyticData, compositeIdentificator, null);
	}
}
