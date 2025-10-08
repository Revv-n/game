using System;
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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SuspendedPaymentsRequest>()).AsSingle();
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<SuspendedPaymentsRequest>(PostRequestType.SuspendedPayments);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<InvoicesFilteredRequest>()).AsSingle();
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
	}

	private void BindRequests()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<StartPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<SuccessPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<FailPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<ServerPaymentNotificator>()).AsSingle();
		((InstallerBase)this).Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<string>)(object)((InstallerBase)this).Container.Bind<string>()).FromResolveGetter<IRequestUrlResolver>((Func<IRequestUrlResolver, string>)((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate)))).WhenInjectedInto(new Type[2]
		{
			typeof(SuccessPaymentRequest),
			typeof(FailPaymentRequest)
		});
	}

	private void BindMonetization()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<NutakuMonetizationPopupOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("nutaku");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
	}

	private void BindFactories()
	{
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<string>)(object)((InstallerBase)this).Container.Bind<string>()).FromResolveGetter<IRequestUrlResolver>((Func<IRequestUrlResolver, string>)((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentCheckout)))).WhenInjectedInto<PaymentFactory>();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PaymentFactory>()).AsSingle();
	}
}
