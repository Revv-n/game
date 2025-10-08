using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization;

public interface IMonetizationAdapter
{
	IObservable<string> OnFailed { get; }

	IObservable<Unit> OnSuccess { get; }

	void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null);
}
