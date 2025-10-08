using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Exceptions.UI;

public class ExceptionHandlerInstaller : MonoInstaller
{
	[SerializeField]
	private DisplayPopupExceptionHandler popupPrefab;

	[SerializeField]
	private Canvas canvas;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<DisplayPopupExceptionHandler>().FromComponentInNewPrefab(popupPrefab).UnderTransform(canvas.transform)
			.AsSingle()
			.OnInstantiated(delegate(InjectContext _context, DisplayPopupExceptionHandler _popup)
			{
				_popup.ExceptionPopupWindow.Canvas = canvas;
			})
			.NonLazy();
	}
}
