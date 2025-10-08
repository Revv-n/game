using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class MonetizationSystem : MonetizationSystem<Transaction>, IMonetizationAdapter
{
	private readonly PaymentController paymentController;

	private readonly PlayerStats playerStats;

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((Transaction _) => default(Unit));

	public MonetizationSystem(PaymentController paymentController, PlayerStats playerStats)
	{
		this.paymentController = paymentController;
		this.playerStats = playerStats;
	}

	public void OnIssueTheProductBought(PaymentIntentData data)
	{
		Transaction value = new Transaction(data.ItemID, isValidated: true, data.ID);
		onSucceeded.OnNext(value);
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		DebugBuy(entityId, price, itemName, itemDescription, itemImageUrl);
		ReactiveProperty<int> checkoutAttemptCount = playerStats.CheckoutAttemptCount;
		int value = checkoutAttemptCount.Value + 1;
		checkoutAttemptCount.Value = value;
		IsValidStringLength(ref itemName, "default_name");
		IsValidStringLength(ref itemDescription, "default_description");
		BuyProduct(lotId, monetizationID, price);
		paymentController.Buy(monetizationID, itemName, entityId, price, itemDescription, itemImageUrl, OnTransactionComplete);
	}

	private void DebugBuy(int itemId, string price, string itemName, string itemDescription, string itemImageURL)
	{
	}

	private void OnTransactionComplete(Transaction _transaction)
	{
		if (_transaction.IsValidated)
		{
			onSucceeded.OnNext(_transaction);
			return;
		}
		onFailed.OnNext("Transaction failed. Transaction Id: " + _transaction.Id + ". " + $" Transaction Status code: {_transaction.StatusCode}" + $" Transaction MonetizationID: {_transaction.monetizationID}");
	}

	private bool IsValidStringLength(ref string inputValue, string defaultValue)
	{
		bool flag = inputValue != null && inputValue.Length <= 64;
		inputValue = (flag ? inputValue : defaultValue);
		return flag;
	}
}
