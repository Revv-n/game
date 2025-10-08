using System;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Monetization.Android.Harem;
using GreenT.HornyScapes.Monetization.Webgl.Harem;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Harem;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string RegionName = "US";

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<MonetizationSystem>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>().AsSingle().WithArguments("US");
		base.Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<OfflinePaymentService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<PopupOpener>().AsSingle();
		base.Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		base.Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		base.Container.Bind<PaymentStatusUpdateRequest>().AsSingle();
		base.Container.Bind<HaremInvoiceStatusRequester>().AsSingle();
		base.Container.Bind<HaremPaymentRequester>().AsSingle();
	}

	private void BindReal()
	{
		base.Container.Bind(typeof(BaseMonetizationSubSystem), typeof(IDisposable)).To<MonetizationSubSystem>().AsSingle();
		base.Container.BindInterfacesTo<GreenT.HornyScapes.Monetization.Webgl.Harem.MonetizationPaymentConnector>().FromNewComponentOnNewGameObject().WithGameObjectName("MonetizationPaymentConnector")
			.AsSingle();
	}

	private void BindRealAndroid()
	{
		base.Container.Bind(typeof(BaseMonetizationSubSystem), typeof(IDisposable)).To<MonetizationSubSystem>().AsSingle();
		base.Container.BindInterfacesTo<GreenT.HornyScapes.Monetization.Android.Harem.MonetizationPaymentConnector>().FromNewComponentOnNewGameObject().WithGameObjectName("MonetizationPaymentConnector")
			.AsSingle();
	}
}
