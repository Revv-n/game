using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyViewInstaller : MonoInstaller
{
	[SerializeField]
	private RestoreEventEnergyView restoreEventEnergyView;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<RestoreEventEnergyView>().FromInstance(restoreEventEnergyView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<RestoreEventEnergyViewSetter>().AsSingle();
	}
}
