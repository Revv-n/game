using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyViewInstaller : MonoInstaller
{
	[SerializeField]
	private RestoreEventEnergyView restoreEventEnergyView;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEventEnergyView>().FromInstance((object)restoreEventEnergyView).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<RestoreEventEnergyViewSetter>()).AsSingle();
	}
}
