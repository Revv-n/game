using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Harem;

public class MonetizationSystem : MonetizationSystem<Transaction>, IMonetizationAdapter
{
	private readonly PlayerStats _playerStats;

	private readonly BaseMonetizationSubSystem _monetizationSubSystem;

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((Transaction _) => default(Unit));

	public MonetizationSystem(PlayerStats playerStats, BaseMonetizationSubSystem monetizationSubSystem)
	{
		_playerStats = playerStats;
		_monetizationSubSystem = monetizationSubSystem;
	}

	public void BuyProduct(string lotId, int monetizationID, int itemId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		ReactiveProperty<int> checkoutAttemptCount = _playerStats.CheckoutAttemptCount;
		int value = checkoutAttemptCount.Value + 1;
		checkoutAttemptCount.Value = value;
		BuyProduct(lotId, monetizationID, price);
		_monetizationSubSystem.Buy(monetizationID, itemName, itemId, price, itemDescription, itemImageUrl, OnTransactionComplete);
	}

	public void OnIssueTheProductBought(PaymentIntentData data)
	{
		Transaction value = new Transaction(data.ItemID, isValidated: true, data.ID);
		onSucceeded.OnNext(value);
	}

	private void OnTransactionComplete(Transaction transaction)
	{
		if (transaction.IsValidated)
		{
			onSucceeded.OnNext(transaction);
			return;
		}
		onFailed.OnNext("It's not valid. Server-InvoiceId: " + transaction.Id + ". " + $" Server response Status code: {transaction.StatusCode}" + $" MonetizationID: {transaction.monetizationID}");
	}
}
