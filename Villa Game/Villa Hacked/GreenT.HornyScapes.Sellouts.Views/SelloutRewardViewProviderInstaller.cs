using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SelloutRewardViewProviderInstaller : MonoInstaller
{
	[SerializeField]
	private RectTransform _rewardContainer;

	[SerializeField]
	private SelloutRewardView _selloutRewardView;

	[SerializeField]
	private RewardViewProvider _rewardViewProvider;

	public override void InstallBindings()
	{
		((FromBinderGeneric<RewardViewProvider>)(object)((MonoInstallerBase)this).Container.Bind<RewardViewProvider>()).FromInstance(_rewardViewProvider).AsSingle();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<SelloutRewardView>()).FromComponentInNewPrefab((Object)_selloutRewardView)).UnderTransform((Transform)_rewardContainer).AsSingle();
	}
}
