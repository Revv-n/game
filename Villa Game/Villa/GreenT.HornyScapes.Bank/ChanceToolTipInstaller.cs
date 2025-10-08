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
		base.Container.Bind<RarityChancesView>().FromInstance(chancesView).AsSingle();
		base.Container.BindIFactory<DropChanceView>().FromComponentInNewPrefab(prefab).UnderTransform(viewContainer)
			.AsCached();
		base.Container.BindInterfacesTo<DropChanceView.Manager>().FromNewComponentOn(viewContainer.gameObject).AsSingle();
	}
}
