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
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("US");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<PopupOpener>()).AsSingle();
		((InstallerBase)this).Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		((InstallerBase)this).Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PaymentStatusUpdateRequest>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<HaremInvoiceStatusRequester>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<HaremPaymentRequester>()).AsSingle();
	}

	private void BindReal()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderNonGeneric)((InstallerBase)this).Container.Bind(new Type[2]
		{
			typeof(BaseMonetizationSubSystem),
			typeof(IDisposable)
		})).To<MonetizationSubSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((InstallerBase)this).Container.BindInterfacesTo<GreenT.HornyScapes.Monetization.Webgl.Harem.MonetizationPaymentConnector>()).FromNewComponentOnNewGameObject().WithGameObjectName("MonetizationPaymentConnector")).AsSingle();
	}

	private void BindRealAndroid()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((ConcreteBinderNonGeneric)((InstallerBase)this).Container.Bind(new Type[2]
		{
			typeof(BaseMonetizationSubSystem),
			typeof(IDisposable)
		})).To<MonetizationSubSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((InstallerBase)this).Container.BindInterfacesTo<GreenT.HornyScapes.Monetization.Android.Harem.MonetizationPaymentConnector>()).FromNewComponentOnNewGameObject().WithGameObjectName("MonetizationPaymentConnector")).AsSingle();
	}
}
