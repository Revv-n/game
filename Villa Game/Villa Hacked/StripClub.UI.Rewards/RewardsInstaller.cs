using GreenT.HornyScapes.UI;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Rewards;

public class RewardsInstaller : MonoInstaller<RewardsInstaller>
{
	[SerializeField]
	private Transform container;

	[SerializeField]
	private CardResourceView resourceViewPrefab;

	[SerializeField]
	private CardView cardViewPrefab;

	public override void InstallBindings()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardView>()).FromComponentInNewPrefab((Object)cardViewPrefab)).UnderTransform(container);
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardResourceView>()).FromComponentInNewPrefab((Object)resourceViewPrefab)).UnderTransform(container);
	}
}
