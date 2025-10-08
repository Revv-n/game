using System;
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
		((NonLazyBinder)((InstantiateCallbackConditionCopyNonLazyBinder)((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.BindInterfacesAndSelfTo<DisplayPopupExceptionHandler>()).FromComponentInNewPrefab((UnityEngine.Object)popupPrefab)).UnderTransform(canvas.transform).AsSingle()).OnInstantiated<DisplayPopupExceptionHandler>((Action<InjectContext, DisplayPopupExceptionHandler>)delegate(InjectContext _context, DisplayPopupExceptionHandler _popup)
		{
			_popup.ExceptionPopupWindow.Canvas = canvas;
		})).NonLazy();
	}
}
