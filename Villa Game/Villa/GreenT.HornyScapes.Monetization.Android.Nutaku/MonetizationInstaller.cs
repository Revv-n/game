using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Monetization.Webgl.Nutaku;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Android.Nutaku;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string RegionName = "nutaku";

	public override void InstallBindings()
	{
		BindFactories();
		BindRequests();
		BindMonetization();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SuspendedPaymentsRequest>().AsSingle();
		base.Container.BindUrlWhenInjectedToType<SuspendedPaymentsRequest>(PostRequestType.SuspendedPayments);
		base.Container.BindInterfacesAndSelfTo<InvoicesFilteredRequest>().AsSingle();
		base.Container.BindUrlWhenInjectedToType<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
	}

	private void BindRequests()
	{
		base.Container.Bind<StartPaymentRequest>().AsSingle();
		base.Container.Bind<SuccessPaymentRequest>().AsSingle();
		base.Container.Bind<FailPaymentRequest>().AsSingle();
		base.Container.Bind<ServerPaymentNotificator>().AsSingle();
		base.Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		base.Container.Bind<string>().FromResolveGetter((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate)).WhenInjectedInto(typeof(SuccessPaymentRequest), typeof(FailPaymentRequest));
	}

	private void BindMonetization()
	{
		base.Container.BindInterfacesAndSelfTo<NutakuMonetizationPopupOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("nutaku");
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
	}

	private void BindFactories()
	{
		base.Container.Bind<string>().FromResolveGetter((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentCheckout)).WhenInjectedInto<PaymentFactory>();
		base.Container.Bind<PaymentFactory>().AsSingle();
	}
}
