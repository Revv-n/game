using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<RegionPriceResolver>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SteamMonetizationPopupOpener>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<MonetizationRecorder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<GiftSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
		BindRequests();
	}

	private void BindRequests()
	{
		base.Container.BindPostRequest<SteamRegionRequest>(PostRequestType.UserInfo);
		base.Container.BindPostRequest<SteamCheckoutRequest>(PostRequestType.PaymentCheckout);
		base.Container.BindPostRequest<SteamConfirmRequest>(PostRequestType.SteamPurchaseConfirm);
		base.Container.BindPostRequest<SteamUnreceivedRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.BindPostRequest<SteamReceivedRequest>(PostRequestType.ReceivedPayment);
	}
}
