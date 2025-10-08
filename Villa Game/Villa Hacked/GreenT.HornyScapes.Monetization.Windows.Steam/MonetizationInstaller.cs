using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<RegionPriceResolver>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SteamMonetizationPopupOpener>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationRecorder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<GiftSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
		BindRequests();
	}

	private void BindRequests()
	{
		((InstallerBase)this).Container.BindPostRequest<SteamRegionRequest>(PostRequestType.UserInfo);
		((InstallerBase)this).Container.BindPostRequest<SteamCheckoutRequest>(PostRequestType.PaymentCheckout);
		((InstallerBase)this).Container.BindPostRequest<SteamConfirmRequest>(PostRequestType.SteamPurchaseConfirm);
		((InstallerBase)this).Container.BindPostRequest<SteamUnreceivedRequest>(PostRequestType.PlayerInvoiceStatus);
		((InstallerBase)this).Container.BindPostRequest<SteamReceivedRequest>(PostRequestType.ReceivedPayment);
	}
}
