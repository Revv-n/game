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
		base.Container.Bind<RewardViewProvider>().FromInstance(_rewardViewProvider).AsSingle();
		base.Container.BindIFactory<SelloutRewardView>().FromComponentInNewPrefab(_selloutRewardView).UnderTransform(_rewardContainer)
			.AsSingle();
	}
}
