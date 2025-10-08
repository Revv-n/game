using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Subscription.Push;
using GreenT.Settings.Data;
using Zenject;

namespace GreenT.HornyScapes.Subscription;

public class SubscriptionCoreInstaller : Installer<SubscriptionCoreInstaller>
{
	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<SubscriptionPushNotifierFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesTo<SubscriptionPushSettingsFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionPushInitializer>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionPushSettings.Manager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SubscriptionPushMapper>>()).AsSingle();
		BindCore();
	}

	private void BindCore()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.Bind<SubscriptionModelFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionService>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((InstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionStorage>()).AsSingle();
		((InstallerBase)this).Container.BindPostRequest<SubscriptionsActiveRequest>(PostRequestType.SubscriptionsActive);
	}
}
