using GreenT.Data;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Content;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class BaseEnergyRestoreFactory<T> : IFactory<T>, IFactory where T : BaseEnergyRestore, new()
{
	private readonly IPurchaseProcessor _purchaseProcessor;

	private readonly CurrencyContentFactory _currencyContentFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	private readonly IConstants<int> _initConstants;

	private readonly ISaver _saver;

	private readonly IClock _clock;

	private readonly CurrencyType _currencyType;

	protected abstract string PriceId { get; }

	protected abstract string MaxPrice { get; }

	protected abstract string PriceStep { get; }

	protected abstract string AddEnergy { get; }

	protected abstract bool IsFreeFirstBuy { get; }

	protected abstract CurrencyAmplitudeAnalytic.SourceType AmplitudeSourceType { get; }

	protected BaseEnergyRestoreFactory(IPurchaseProcessor purchaseProcessor, CurrencyContentFactory currencyContentFactory, LinkedContentAnalyticDataFactory analyticDataFactory, IConstants<int> initConstants, ISaver saver, IClock clock, CurrencyType currencyType)
	{
		_purchaseProcessor = purchaseProcessor;
		_analyticDataFactory = analyticDataFactory;
		_currencyContentFactory = currencyContentFactory;
		_initConstants = initConstants;
		_saver = saver;
		_clock = clock;
		_currencyType = currencyType;
	}

	public T Create()
	{
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(AmplitudeSourceType);
		CurrencyLinkedContent energyContent = _currencyContentFactory.Create(_initConstants[AddEnergy], _currencyType, analyticData, default(CompositeIdentificator));
		ResetDailyPriceLogics priceLogics = new ResetDailyPriceLogics(_initConstants[PriceId], _initConstants[PriceStep], _initConstants[MaxPrice], _clock, IsFreeFirstBuy);
		T val = new T();
		val.SetArguments(_purchaseProcessor, energyContent, priceLogics, _currencyType);
		RestoreInitialization(val);
		_saver.Add(val);
		return val;
	}

	protected virtual void RestoreInitialization(T energyRestore)
	{
		energyRestore.Initialization();
	}
}
