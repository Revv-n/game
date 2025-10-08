using GreenT.HornyScapes.Extensions;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTaskRootViewInstaller : MonoInstaller
{
	[SerializeField]
	private MiniEventTaskItemView _taskTemplate;

	[SerializeField]
	private Transform _tasksRoot;

	[SerializeField]
	private MiniEventTaskItemViewManager _miniEventTaskItemViewManager;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<MiniEventTaskItemViewManager>().FromInstance(_miniEventTaskItemViewManager).AsSingle();
		base.Container.BindViewFactory<MiniEventTask, MiniEventTaskItemView>(_tasksRoot, _taskTemplate);
	}
}
