using DG.Tweening;
using GreenT.HornyScapes.Meta;
using GreenT.HornyScapes.ToolTips;
using TMPro;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

public class JarvisGirlToolTipView : ToolTipView<ToolTipSettings>
{
	[SerializeField]
	private AbstractZoomScaler zoomScaler;

	[SerializeField]
	private TextMeshProUGUI text;

	private Sequence showSequence;

	private void Awake()
	{
		zoomScaler.StartScaling();
	}

	public override void Set(ToolTipSettings settings)
	{
		base.Set(settings);
		base.transform.localPosition = settings.ToolTipPosition;
		text.text = settings.KeyText;
	}
}
