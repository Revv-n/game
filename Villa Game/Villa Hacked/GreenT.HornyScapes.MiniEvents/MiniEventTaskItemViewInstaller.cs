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
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTaskItemRewardViewManager>().FromInstance((object)_miniEventTaskItemRewardViewManager).AsSingle();
		((MonoInstallerBase)this).Container.BindViewFactory<LinkedContent, MiniEventTaskLootboxRewardItemView>(_rewardsRoot, _lootboxRewardTemplate);
		((MonoInstallerBase)this).Container.BindViewFactory<LinkedContent, MiniEventTaskCurrencyRewardItemView>(_rewardsRoot, _currencyRewardTemplate);
		((MonoInstallerBase)this).Container.BindViewFactory<LinkedContent, MiniEventTaskCharactersRewardItemView>(_rewardsRoot, _charactersRewardTemplate);
		((MonoInstallerBase)this).Container.BindViewFactory<LinkedContent, MiniEventTaskDecorationRewardItemView>(_rewardsRoot, _decorationRewardTemplate);
		((MonoInstallerBase)this).Container.BindViewFactory<LinkedContent, MiniEventTaskMergeRewardItemView>(_rewardsRoot, _mergeRewardTemplate);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesTo<MiniEventTaskRewardFactory>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<WindowArgumentApplier>()).AsSingle();
	}
}
