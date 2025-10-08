using GreenT.HornyScapes.BattlePassSpace.RewardCards;
using GreenT.HornyScapes.BattlePassSpace.UI.MetaWindow;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class BattlePassMainInstaller : MonoInstaller<EventInstaller>
{
	public BattlePassProgressView battlePassProgressView;

	public EventBattlePassViewer eventBattlePassViewer;

	[Header("Для пуша. Открыть окно прогресса на мете")]
	public WindowOpener StartWindowOpener;

	[Header("EventWindowOpener")]
	public WindowID StartWindowID;

	public WindowID ProgressWindowID;

	public WindowOpener MetaWindowOpenerFromMerge;

	[SerializeField]
	private BattlePassRewardHolderFactory _holderFactoryPrefab;

	[SerializeField]
	private BattlePassRewardLevelView Prefab;

	[SerializeField]
	private Transform Root;

	public override void InstallBindings()
	{
		((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassRewardLevelManager>()).FromNewComponentOn(Root.gameObject).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassRewardHolderFactory>().FromInstance((object)_holderFactoryPrefab).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassRewardLevelViewFactory>()).AsSingle()).WithArguments<Transform, BattlePassRewardLevelView>(Root, Prefab);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassProgressView>().FromInstance((object)battlePassProgressView).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassPushController>()).AsCached()).WithArguments<WindowID>(StartWindowID);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassCalendarCompleteSubsystem>()).AsCached();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<BattlePassRewardedWindowOpener>()).AsSingle()).WithArguments<WindowID, WindowID>(StartWindowID, ProgressWindowID);
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventBattlePassViewer>().FromInstance((object)eventBattlePassViewer).AsSingle();
	}
}
