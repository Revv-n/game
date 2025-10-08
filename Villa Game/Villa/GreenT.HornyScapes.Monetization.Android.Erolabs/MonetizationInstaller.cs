using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string RegionName = "erolabs";

	public override void InstallBindings()
	{
		BindFactories();
		BindRequests();
		BindMonetization();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SuspendedPaymentsRequest>().AsSingle();
		base.Container.BindUrlWhenInjectedToType<SuspendedPaymentsRequest>(PostRequestType.SuspendedPayments);
	}

	private void BindRequests()
	{
		base.Container.BindPostRequest<ErolabsCheckoutRequest>(PostRequestType.PaymentCheckout);
		base.Container.BindPostRequest<ErolabsGetBalanceRequest>(PostRequestType.ErolabsGetBalanceRequest);
		base.Container.BindPostRequest<ErolabsInvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.BindPostRequest<InvoicesFilteredRequestNoResponse>(PostRequestType.PlayerInvoiceStatus);
		base.Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		base.Container.Bind<StartPaymentRequest>().AsSingle();
		base.Container.Bind<SuccessPaymentRequest>().AsSingle();
		base.Container.Bind<FailPaymentRequest>().AsSingle();
		base.Container.Bind<ServerPaymentNotificator>().AsSingle();
		base.Container.Bind<PaymentStatusUpdateRequest>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("erolabs");
		base.Container.Bind<string>().FromResolveGetter((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate)).WhenInjectedInto(typeof(SuccessPaymentRequest), typeof(FailPaymentRequest));
	}

	private void BindMonetization()
	{
		base.Container.BindInterfacesAndSelfTo<ErolabsMonetizationPopupOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.Bind<PaymentFactory>().AsSingle();
	}
}
