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
		base.Container.Bind<SubscriptionPushNotifierFactory>().AsSingle();
		base.Container.BindInterfacesTo<SubscriptionPushSettingsFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionPushInitializer>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionPushSettings.Manager>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<StructureInitializerProxyWithArrayFromConfig<SubscriptionPushMapper>>().AsSingle();
		BindCore();
	}

	private void BindCore()
	{
		base.Container.Bind<SubscriptionModelFactory>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionService>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionStorage>().AsSingle();
		base.Container.BindPostRequest<SubscriptionsActiveRequest>(PostRequestType.SubscriptionsActive);
	}
}
