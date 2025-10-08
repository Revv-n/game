using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Tasks.UI;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskItemViewInstaller : MonoInstaller
{
	[SerializeField]
	private MiniEventTaskLootboxRewardItemView _lootboxRewardTemplate;

	[SerializeField]
	private MiniEventTaskCurrencyRewardItemView _currencyRewardTemplate;

	[SerializeField]
	private MiniEventTaskCharactersRewardItemView _charactersRewardTemplate;

	[SerializeField]
	private MiniEventTaskDecorationRewardItemView _decorationRewardTemplate;

	[SerializeField]
	private MiniEventTaskMergeRewardItemView _mergeRewardTemplate;

	[SerializeField]
	private Transform _rewardsRoot;

	[SerializeField]
	private MiniEventTaskItemRewardViewManager _miniEventTaskItemRewardViewManager;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventTaskItemRewardViewManager>().FromInstance(_miniEventTaskItemRewardViewManager).AsSingle();
		base.Container.BindViewFactory<LinkedContent, MiniEventTaskLootboxRewardItemView>(_rewardsRoot, _lootboxRewardTemplate);
		base.Container.BindViewFactory<LinkedContent, MiniEventTaskCurrencyRewardItemView>(_rewardsRoot, _currencyRewardTemplate);
		base.Container.BindViewFactory<LinkedContent, MiniEventTaskCharactersRewardItemView>(_rewardsRoot, _charactersRewardTemplate);
		base.Container.BindViewFactory<LinkedContent, MiniEventTaskDecorationRewardItemView>(_rewardsRoot, _decorationRewardTemplate);
		base.Container.BindViewFactory<LinkedContent, MiniEventTaskMergeRewardItemView>(_rewardsRoot, _mergeRewardTemplate);
		base.Container.BindInterfacesTo<MiniEventTaskRewardFactory>().AsSingle();
		base.Container.Bind<WindowArgumentApplier>().AsSingle();
	}
}
