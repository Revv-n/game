using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public class LocalMonetizationAdapter : IMonetizationAdapter, IDisposable
{
	private Subject<string> onFailed = new Subject<string>();

	private Subject<Unit> onSuccess = new Subject<Unit>();

	public IObservable<string> OnFailed => onFailed;

	public IObservable<Unit> OnSuccess => onSuccess;

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
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
