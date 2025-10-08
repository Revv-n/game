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
		((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MiniEventTaskItemViewManager>().FromInstance((object)_miniEventTaskItemViewManager).AsSingle();
		((MonoInstallerBase)this).Container.BindViewFactory<MiniEventTask, MiniEventTaskItemView>(_tasksRoot, _taskTemplate);
	}
}
