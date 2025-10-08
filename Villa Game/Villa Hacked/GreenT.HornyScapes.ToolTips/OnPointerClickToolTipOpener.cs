using System;
using GreenT.HornyScapes.Constants;
using StripClub.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class OnPointerClickToolTipOpener<TSettings, TView> : AbstractToolTipOpener<TSettings, TView>, IPointerClickHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler where TSettings : ToolTipSettings where TView : MonoView<TSettings>
{
	private bool isDragging;

	[Inject]
	private void Init(IConstants<int> intConstants)
	{
		showTime = TimeSpan.FromSeconds(intConstants["phrase_hiring"]);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if ((bool)base.Settings && !base.IsPlaying.Value && !isDragging)
		{
			OpenToolTip(base.Settings);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		isDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
	}
}
