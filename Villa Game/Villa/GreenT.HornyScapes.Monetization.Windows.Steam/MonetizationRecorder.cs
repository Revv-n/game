using System;
using GreenT.HornyScapes.Saves;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class MonetizationRecorder : IMonetizationRecorder<SteamPaymentData>, IMonetizationRecorder
{
	private readonly SteamReceivedRequest _receivedRequest;

	private readonly SaveNotifier _saveNotifier;

	private SteamPaymentData _steamPaymentData;

	public MonetizationRecorder(SteamReceivedRequest receivedRequest, SaveNotifier saveNotifier)
	{
		_receivedRequest = receivedRequest;
		_saveNotifier = saveNotifier;
	}

	public void Record(SteamPaymentData data)
	{
		_steamPaymentData = data;
	}

	public IObservable<Unit> Approve(SteamPaymentData data)
	{
		return _saveNotifier.OnSave().Take(1).SelectMany((Unit _) => _receivedRequest.Post(data))
			.AsUnitObservable();
	}

	public IObservable<Unit> ApproveLast()
	{
		return Approve(_steamPaymentData);
	}
}
