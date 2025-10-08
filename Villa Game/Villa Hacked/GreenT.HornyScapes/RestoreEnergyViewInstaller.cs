using GreenT.HornyScapes.Bank.GoldenTickets.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEnergyViewInstaller : MonoInstaller
{
	[SerializeField]
	private RestoreEnergyView restoreEnergyView;

	[SerializeField]
	private GoldenTicketView goldenTicketView;

	[SerializeField]
	private SubscriptionGoldenTicketView _subscriptionGoldenTicketView;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEnergyView>().FromInstance((object)restoreEnergyView).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<GoldenTicketView>().FromInstance((object)goldenTicketView).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<SubscriptionGoldenTicketView>().FromInstance((object)_subscriptionGoldenTicketView).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEnergyViewSetter>()).AsSingle();
	}
}
