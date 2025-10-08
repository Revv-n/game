using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class LocalMonetizationAdapter : IMonetizationAdapter, IDisposable
{
	private Subject<string> onFailed = new Subject<string>();

	private Subject<Unit> onSuccess = new Subject<Unit>();

	public IObservable<string> OnFailed => (IObservable<string>)onFailed;

	public IObservable<Unit> OnSuccess => (IObservable<Unit>)onSuccess;

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		onSuccess.OnNext(Unit.Default);
	}

	public void Dispose()
	{
		onFailed.OnCompleted();
		onFailed.Dispose();
		onSuccess.OnCompleted();
		onSuccess.Dispose();
	}
}
