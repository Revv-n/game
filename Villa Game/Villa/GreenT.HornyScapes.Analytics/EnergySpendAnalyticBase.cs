using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes.Analytics;

public class EnergySpendAnalyticBase : BaseEntityAnalytic<int>
{
	private readonly CurrencyAmplitudeAnalytic _currencyAmplitudeAnalytic;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly CurrencyType _currencyType;

	private readonly ContentType _contentType;

	public EnergySpendAnalyticBase(IAmplitudeSender<AmplitudeEvent> amplitude, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor, CurrencyType currencyType, ContentType contentType)
		: base(amplitude)
	{
		_currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		_currencyProcessor = currencyProcessor;
		_currencyType = currencyType;
		_contentType = contentType;
	}

	public override void Track()
	{
		ClearStreams();
		TrackAnyPromote();
	}

	private void TrackAnyPromote()
	{
		_currencyProcessor.GetSpendStream(_currencyType, SendEventByPass).AddTo(onNewStream);
	}

	public override void SendEventByPass(int value)
	{
		_currencyAmplitudeAnalytic.SendSpentEvent(_currencyType, value, CurrencyAmplitudeAnalytic.SourceType.SpawnMergeItem, _contentType);
	}
}
