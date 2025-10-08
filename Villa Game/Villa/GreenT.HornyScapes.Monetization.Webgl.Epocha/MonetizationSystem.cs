using System;
using UniRx;

namespace GreenT.HornyScapes.Monetization.Webgl.Epocha;

public class MonetizationSystem : MonetizationSystem<PaymentIntentData>, IMonetizationAdapter
{
	private readonly MonetizationSubsystem subsystem;

	public override IObservable<string> OnFailed => subsystem.OnFailed;

	public IObservable<Unit> OnSuccess => OnSucceeded.Select((PaymentIntentData _) => default(Unit));

	public override IObservable<PaymentIntentData> OnSucceeded => subsystem.OnCompleted;

	public override IObservable<CheckoutData> OnOpenForm => subsystem.OnOpenForm;

	public MonetizationSystem(MonetizationSubsystem monetizationSubsystem)
	{
		subsystem = monetizationSubsystem;
	}

	public void BuyProduct(string lotId, int monetizationID, int entityId, string price = null, string region = null, string itemName = null, string itemDescription = null, string itemImageUrl = null, string currenyType = null)
	{
		BuyProduct(lotId, monetizationID, price);
	}

	protected override void BuyProduct(Product product)
	{
		subsystem.BuyProduct(product);
		base.BuyProduct(product);
	}
}
