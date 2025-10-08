using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class ChanceToolTipInstaller : MonoInstaller<ChanceToolTipInstaller>
{
	[SerializeField]
	private RarityChancesView chancesView;

	[SerializeField]
	private DropChanceView prefab;

	[SerializeField]
	private Transform viewContainer;

	public override void InstallBindings()
	{
		((FromBinderGeneric<RarityChancesView>)(object)((MonoInstallerBase)this).Container.Bind<RarityChancesView>()).FromInstance(chancesView).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<DropChanceView>()).FromComponentInNewPrefab((Object)prefab)).UnderTransform(viewContainer).AsCached();
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<DropChanceView.Manager>()).FromNewComponentOn(viewContainer.gameObject).AsSingle();
	}
}
