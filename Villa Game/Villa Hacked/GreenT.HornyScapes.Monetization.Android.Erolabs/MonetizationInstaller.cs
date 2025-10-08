using System;
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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SuspendedPaymentsRequest>()).AsSingle();
		((InstallerBase)this).Container.BindUrlWhenInjectedToType<SuspendedPaymentsRequest>(PostRequestType.SuspendedPayments);
	}

	private void BindRequests()
	{
		((InstallerBase)this).Container.BindPostRequest<ErolabsCheckoutRequest>(PostRequestType.PaymentCheckout);
		((InstallerBase)this).Container.BindPostRequest<ErolabsGetBalanceRequest>(PostRequestType.ErolabsGetBalanceRequest);
		((InstallerBase)this).Container.BindPostRequest<ErolabsInvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		((InstallerBase)this).Container.BindPostRequest<InvoicesFilteredRequestNoResponse>(PostRequestType.PlayerInvoiceStatus);
		((InstallerBase)this).Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		((InstallerBase)this).Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<StartPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<SuccessPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<FailPaymentRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<ServerPaymentNotificator>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PaymentStatusUpdateRequest>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("erolabs");
		((ConditionCopyNonLazyBinder)((FromBinderGeneric<string>)(object)((InstallerBase)this).Container.Bind<string>()).FromResolveGetter<IRequestUrlResolver>((Func<IRequestUrlResolver, string>)((IRequestUrlResolver _resolver) => _resolver.PostRequestUrl(PostRequestType.PaymentStatusUpdate)))).WhenInjectedInto(new Type[2]
		{
			typeof(SuccessPaymentRequest),
			typeof(FailPaymentRequest)
		});
	}

	private void BindMonetization()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<ErolabsMonetizationPopupOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
	}

	private void BindFactories()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PaymentFactory>()).AsSingle();
	}
}
