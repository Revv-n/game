namespace GreenT.HornyScapes.Monetization.Harem;

public class DummyMonetizationSubSystem : BaseMonetizationSubSystem
{
	public DummyMonetizationSubSystem(IPaymentConnector paymentConnector)
		: base(paymentConnector)
	{
	}

	protected override void OnPurchaseComplete(int monetizationId)
	{
		Transaction transaction = _transactions[monetizationId];
		transaction.Id = GetTransactionId();
		transaction.IsValidated = true;
		_transactionCallback(transaction);
	}

	protected override string GetTransactionId()
	{
		return "Fake";
	}
}
