using System;
using GreenT.HornyScapes.Extensions;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class MonetizationInstaller : Installer<MonetizationInstaller>
{
	private const string JS_CONTROLLER_NAME = "NutakuTestController";

	private const string RegionName = "nutaku";

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<Connection>()).AsSingle();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((InstallerBase)this).Container.Bind<PaymentController>()).FromNewComponentOnNewGameObject()).AsSingle()).OnInstantiated<PaymentController>((Action<InjectContext, PaymentController>)delegate(InjectContext _context, PaymentController _object)
		{
			_object.gameObject.name = "NutakuTestController";
		})).NonLazy();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultRegionPriceResolver>()).AsSingle()).WithArguments<string>("nutaku");
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<MonetizationSystem>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<DefaultMonetizationRecorder>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<OfflinePaymentService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<NutakuMonetizationPopupOpener>()).AsSingle();
		((InstallerBase)this).Container.BindPostRequest<ReceivedRequest>(PostRequestType.ReceivedPayment);
		((InstallerBase)this).Container.BindPostRequest<InvoicesFilteredRequest>(PostRequestType.PlayerInvoiceStatus);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<PaymentStatusUpdateRequest>()).AsSingle();
	}
}
