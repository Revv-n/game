using System;
using GreenT.HornyScapes.Meta.Navigation;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class OnPointerDownToolTipOpener<TSettings, TView> : AbstractToolTipOpener<TSettings, TView>, IPointerClickHandler, IEventSystemHandler where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	private IPointerPress pointerPress;

	[Inject]
	public void Init(IPointerPress pointerPress)
	{
		this.pointerPress = pointerPress;
	}

	public virtual void OnPointerClick(PointerEventData eventData)
	{
		if (!base.IsPlaying.Value)
		{
			OpenToolTip(base.Settings);
		}
	}

	protected override IObservable<Unit> ShutDownObservable()
	{
		return Observable.Take<Unit>(Observable.Merge<Unit>(base.ShutDownObservable(), new IObservable<Unit>[1] { Observable.AsUnitObservable<Vector2>(Observable.TakeUntil<Vector2, bool>(Observable.Skip<Vector2>(pointerPress.OnPointerClick(), 1), Observable.First<bool>((IObservable<bool>)base.IsPlaying, (Func<bool, bool>)((bool x) => !x)))) }), 1);
	}
}
