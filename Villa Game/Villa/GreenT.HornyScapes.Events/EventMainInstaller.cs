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
		base.Container.BindInterfacesAndSelfTo<EventProgressView>().FromInstance(eventProgressView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventStartView>().FromInstance(eventStartView).AsSingle();
		base.Container.BindInterfacesAndSelfTo<EventPushController>().AsCached().WithArguments(StartWindowOpener);
		base.Container.BindInterfacesAndSelfTo<EventCalendarCompleteSubsystem>().AsCached();
		base.Container.BindInterfacesAndSelfTo<EventWindowOpener>().AsSingle().WithArguments(StartWindowID, ProgressWindowID, MetaWindowOpenerFromMerge);
	}
}
