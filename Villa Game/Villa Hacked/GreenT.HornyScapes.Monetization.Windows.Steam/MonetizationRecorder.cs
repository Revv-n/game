using System;
using GreenT.HornyScapes.Saves;
using GreenT.Net;
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
		return Observable.AsUnitObservable<Response>(Observable.SelectMany<Unit, Response>(Observable.Take<Unit>(_saveNotifier.OnSave(), 1), (Func<Unit, IObservable<Response>>)((Unit _) => _receivedRequest.Post(data))));
	}

	public IObservable<Unit> ApproveLast()
	{
		return Approve(_steamPaymentData);
	}
}
