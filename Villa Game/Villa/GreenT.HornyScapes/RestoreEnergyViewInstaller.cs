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
		base.Container.BindInterfacesAndSelfTo<RestoreEnergyView>().FromInstance(restoreEnergyView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<GoldenTicketView>().FromInstance(goldenTicketView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<SubscriptionGoldenTicketView>().FromInstance(_subscriptionGoldenTicketView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEnergyViewSetter>().AsSingle();
	}
}
