using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventMainInstaller : MonoInstaller<EventInstaller>
{
	public EventProgressView eventProgressView;

	public EventStartView eventStartView;

	[Header("Для пуша. Открыть окно прогресса на мете")]
	public WindowOpener StartWindowOpener;

	[Header("EventWindowOpener")]
	public WindowID StartWindowID;

	public WindowID ProgressWindowID;

	public WindowOpener MetaWindowOpenerFromMerge;

	public override void InstallBindings()
	{
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventProgressView>().FromInstance((object)eventProgressView).AsSingle();
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventStartView>().FromInstance((object)eventStartView).AsSingle();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventPushController>()).AsCached()).WithArguments<WindowOpener>(StartWindowOpener);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventCalendarCompleteSubsystem>()).AsCached();
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<EventWindowOpener>()).AsSingle()).WithArguments<WindowID, WindowID, WindowOpener>(StartWindowID, ProgressWindowID, MetaWindowOpenerFromMerge);
	}
}
