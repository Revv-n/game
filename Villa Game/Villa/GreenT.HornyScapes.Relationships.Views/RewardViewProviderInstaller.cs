using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Views;

public class RewardViewProviderInstaller : MonoInstaller
{
	[SerializeField]
	private LootboxRewardView _lootboxRewardView;

	[SerializeField]
	private DateRewardView _dateRewardView;

	[SerializeField]
	private ComingSoonDateRewardView _comingSoonDateRewardView;

	[SerializeField]
	private RewardViewProvider _rewardViewProvider;

	public override void InstallBindings()
	{
		base.Container.Bind<RewardViewProvider>().FromInstance(_rewardViewProvider).AsSingle();
		BindViewFactory(_lootboxRewardView);
		BindViewFactory(_dateRewardView);
		BindViewFactory(_comingSoonDateRewardView);
	}

	private void BindViewFactory<TView>(TView rewardView) where TView : MonoView
	{
		base.Container.BindIFactory<TView>().FromComponentInNewPrefab(rewardView).AsSingle();
	}
}
