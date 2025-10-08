using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.Net;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class Connection : IDisposable
{
	private Action<Transaction> m_TransactionCallback;

	private readonly ReceivedRequest receivedRequest;

	private readonly IDataStorage dataStorage;

	private CompositeDisposable recievedRequests;

	private Dictionary<int, Transaction> transactions;

	public Connection(ReceivedRequest receivedRequest, IDataStorage dataStorage)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		this.receivedRequest = receivedRequest;
		this.dataStorage = dataStorage;
		recievedRequests = new CompositeDisposable();
		transactions = new Dictionary<int, Transaction>();
	}

	public void Buy(int monetizationID, string itemName, int itemId, string price, string description, string imageUrl, Action<Transaction> onTransactionComplete)
	{
		m_TransactionCallback = onTransactionComplete;
		Transaction value = new Transaction(monetizationID, isValidated: false);
		transactions[monetizationID] = value;
	}

	private string GetTransactionId()
	{
		throw new NotImplementedException();
	}

	private void FakeValidation(Transaction transaction)
	{
		transaction.Id = GetTransactionId();
		transaction.IsValidated = true;
		m_TransactionCallback(transaction);
	}

	public void OnPurchaseComplete(int monetizationID)
	{
		Transaction transaction = transactions[monetizationID];
		transaction.Id = GetTransactionId();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(NotifyAboutSuccessfulTransaction(transaction), (Action<Response>)delegate(Response _response)
		{
			transaction.StatusCode = _response.Status;
			transaction.IsValidated = IsValidStatus(_response.Status);
			m_TransactionCallback(transaction);
		}), (ICollection<IDisposable>)recievedRequests);
	}

	private IObservable<Response> NotifyAboutSuccessfulTransaction(Transaction transaction)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>();
		return receivedRequest.Post(fields, transaction.Id);
	}

	private bool IsValidStatus(int status)
	{
		if (status != 0)
		{
			return status == 200;
		}
		return true;
	}

	public void OnPurchaseFailed(int monetizationID, string message)
	{
		Transaction transaction = transactions[monetizationID];
		transaction.IsValidated = false;
		transaction.Id = string.Empty;
		_ = "Purchase failed. Message: " + message + ". ";
		m_TransactionCallback(transaction);
	}

	public virtual void Dispose()
	{
		recievedRequests.Dispose();
	}
}
