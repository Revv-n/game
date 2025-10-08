using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes.Monetization.Harem;

public abstract class BaseMonetizationSubSystem
{
	protected Action<Transaction> _transactionCallback;

	protected Dictionary<int, Transaction> _transactions;

	private readonly IPaymentConnector _paymentConnector;

	public BaseMonetizationSubSystem(IPaymentConnector paymentConnector)
	{
		_transactions = new Dictionary<int, Transaction>();
		_paymentConnector = paymentConnector;
	}

	public void Buy(int monetizationID, string itemName, int itemId, string price, string itemDescription, string itemImageURL, Action<Transaction> onTransactionComplete)
	{
		_transactionCallback = onTransactionComplete;
		Transaction value = new Transaction(monetizationID, isValidated: false);
		_transactions[monetizationID] = value;
		_paymentConnector.Connect(monetizationID, itemName, price, itemDescription, itemImageURL, OnPurchaseComplete, OnPurchaseFailed);
	}

	protected abstract void OnPurchaseComplete(int monetizationId);

	protected virtual void OnPurchaseFailed(int monetizationId)
	{
		Transaction transaction = _transactions[monetizationId];
		transaction.IsValidated = false;
		transaction.Id = GetTransactionId() ?? string.Empty;
		_transactionCallback(transaction);
	}

	protected abstract string GetTransactionId();
}
