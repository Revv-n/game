using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Epocha;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string RegionName = "US";

	public override void InstallBindings()
	{
		BindRequests();
		BindSubSystems();
		BindMonetization();
	}

	private void BindMonetization()
	{
		base.Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("US");
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
	}

	private void BindRequests()
	{
		base.Container.BindPostRequest<CheckoutRequest>(PostRequestType.PaymentCheckout);
		base.Container.BindPostRequest<PaymentIntentRequest>(PostRequestType.PaymentIntent);
		base.Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		base.Container.BindPostRequest<AbortRequest>(PostRequestType.PaymentStatusUpdate);
	}

	private void BindSubSystems()
	{
		base.Container.BindInterfacesAndSelfTo<MonetizationSubsystem>().AsSingle();
	}
}
