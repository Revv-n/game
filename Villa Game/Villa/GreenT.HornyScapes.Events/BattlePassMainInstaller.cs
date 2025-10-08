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
		base.Container.BindInterfacesAndSelfTo<BattlePassRewardLevelManager>().FromNewComponentOn(Root.gameObject).AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassRewardHolderFactory>().FromInstance(_holderFactoryPrefab).AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassRewardLevelViewFactory>().AsSingle().WithArguments(Root, Prefab);
		base.Container.BindInterfacesAndSelfTo<BattlePassProgressView>().FromInstance(battlePassProgressView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<BattlePassPushController>().AsCached().WithArguments(StartWindowID);
		base.Container.BindInterfacesAndSelfTo<BattlePassCalendarCompleteSubsystem>().AsCached();
		base.Container.BindInterfacesAndSelfTo<BattlePassRewardedWindowOpener>().AsSingle().WithArguments(StartWindowID, ProgressWindowID);
		base.Container.BindInterfacesAndSelfTo<EventBattlePassViewer>().FromInstance(eventBattlePassViewer).AsSingle();
	}
}
