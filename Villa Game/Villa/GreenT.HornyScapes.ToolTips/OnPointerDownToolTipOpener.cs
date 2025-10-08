using System;
using GreenT.HornyScapes.Meta.Navigation;
using StripClub.UI;
using UniRx;
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
		return base.ShutDownObservable().Merge(pointerPress.OnPointerClick().Skip(1).TakeUntil(base.IsPlaying.First((bool x) => !x))
			.AsUnitObservable()).Take(1);
	}
}
