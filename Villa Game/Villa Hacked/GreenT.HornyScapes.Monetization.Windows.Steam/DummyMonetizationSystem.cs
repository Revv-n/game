using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class DummyMonetizationSystem : MonetizationSystem<SteamPaymentData>, IMonetizationAdapter
{
	public IObservable<Unit> OnSuccess => Observable.Select<SteamPaymentData, Unit>(OnSucceeded, (Func<SteamPaymentData, Unit>)((SteamPaymentData _) => default(Unit)));

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		onSucceeded.OnNext(new SteamPaymentData());
	}
}
