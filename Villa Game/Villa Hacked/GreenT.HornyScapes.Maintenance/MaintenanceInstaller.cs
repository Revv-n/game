using System;
using GreenT.HornyScapes.Maintenance.UI;
using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Maintenance;

public class MaintenanceInstaller : MonoInstaller
{
	private const int RequestIntervalTime = 42;

	[SerializeField]
	private MaintenancePopupSetter popupPrefab;

	[SerializeField]
	private Canvas canvas;

	public WindowGroupID MTWindowGroup;

	public override void InstallBindings()
	{
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MaintenanceListener>()).AsSingle()).WithArguments<int>(42);
		((ArgConditionCopyNonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MaintenanceWindowOpener>()).AsSingle()).WithArguments<WindowGroupID>(MTWindowGroup);
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DisplayMaintenancePopup>()).AsSingle();
		((NonLazyBinder)((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<VersionProvider>()).AsSingle()).NonLazy();
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<MaintenancePopupSetter>()).FromComponentInNewPrefab((UnityEngine.Object)popupPrefab)).UnderTransform(canvas.transform).AsSingle()).OnInstantiated<MaintenancePopupSetter>((Action<InjectContext, MaintenancePopupSetter>)delegate(InjectContext _context, MaintenancePopupSetter _popup)
		{
			_popup.Window.Canvas = canvas;
		})).NonLazy();
	}
}
