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
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("US");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
	}

	private void BindRequests()
	{
		((InstallerBase)this).Container.BindPostRequest<CheckoutRequest>(PostRequestType.PaymentCheckout);
		((InstallerBase)this).Container.BindPostRequest<PaymentIntentRequest>(PostRequestType.PaymentIntent);
		((InstallerBase)this).Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		((InstallerBase)this).Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		((InstallerBase)this).Container.BindPostRequest<AbortRequest>(PostRequestType.PaymentStatusUpdate);
	}

	private void BindSubSystems()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSubsystem>()).AsSingle();
	}
}
