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
		base.Container.BindInterfacesAndSelfTo<MaintenanceListener>().AsSingle().WithArguments(42);
		base.Container.BindInterfacesAndSelfTo<MaintenanceWindowOpener>().AsSingle().WithArguments(MTWindowGroup);
		base.Container.BindInterfacesAndSelfTo<DisplayMaintenancePopup>().AsSingle();
		base.Container.BindInterfacesAndSelfTo<VersionProvider>().AsSingle().NonLazy();
		base.Container.BindInterfacesAndSelfTo<MaintenancePopupSetter>().FromComponentInNewPrefab(popupPrefab).UnderTransform(canvas.transform)
			.AsSingle()
			.OnInstantiated(delegate(InjectContext _context, MaintenancePopupSetter _popup)
			{
				_popup.Window.Canvas = canvas;
			})
			.NonLazy();
	}
}
