using System;
using System.Collections.Generic;
using GreenT.Data;
using GreenT.Net;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Harem;

public class MonetizationSubSystem : BaseMonetizationSubSystem, IDisposable
{
	private readonly ReceivedRequest _receivedRequest;

	private readonly IDataStorage _dataStorage;

	private readonly CompositeDisposable _requests = new CompositeDisposable();

	public MonetizationSubSystem(IPaymentConnector paymentConnector, ReceivedRequest receivedRequest, IDataStorage dataStorage)
		: base(paymentConnector)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_receivedRequest = receivedRequest;
		_dataStorage = dataStorage;
	}

	protected override void OnPurchaseComplete(int monetizationId)
	{
		Transaction transaction = _transactions[monetizationId];
		transaction.Id = GetTransactionId();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Response>(NotifyAboutSuccessfulTransaction(transaction), (Action<Response>)delegate(Response response)
		{
			transaction.StatusCode = response.Status;
			transaction.IsValidated = IsValidStatus(response.Status);
			_transactionCallback(transaction);
		}), (ICollection<IDisposable>)_requests);
	}

	private IObservable<Response> NotifyAboutSuccessfulTransaction(Transaction transaction)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>();
		return _receivedRequest.Post(fields, transaction.Id);
	}

	private bool IsValidStatus(int status)
	{
		if (status != 0)
		{
			return status == 200;
		}
		return true;
	}

	protected override string GetTransactionId()
	{
		return _dataStorage.GetString("lastInvoiceID");
	}

	public void Dispose()
	{
		_requests.Dispose();
	}
}
