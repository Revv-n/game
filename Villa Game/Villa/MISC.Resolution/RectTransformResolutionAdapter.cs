using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MISC.Resolution;

public class RectTransformResolutionAdapter : ResolutionAdapter
{
	[Serializable]
	public class RTResolutionPreset : ResolutionPreset
	{
		public Vector2 anchPosition;

		public Vector2 sizeDelta;

		public Vector2 pivot = new Vector2(0.5f, 0.5f);

		public Vector2 anchMin = new Vector2(0.5f, 0.5f);

		public Vector2 anchMax = new Vector2(0.5f, 0.5f);
	}

	[SerializeField]
	private List<RTResolutionPreset> presets = new List<RTResolutionPreset>(new RTResolutionPreset[3]
	{
		new RTResolutionPreset
		{
			type = ResolutionType.Wide
		},
		new RTResolutionPreset
		{
			type = ResolutionType.Old
		},
		new RTResolutionPreset
		{
			type = ResolutionType.IPad
		}
	});

	public override void Adaptate(ResolutionType type)
	{
		RTResolutionPreset rTResolutionPreset = presets.First((RTResolutionPreset x) => x.type == type);
		RectTransform obj = base.transform as RectTransform;
		obj.pivot = rTResolutionPreset.pivot;
		obj.sizeDelta = rTResolutionPreset.sizeDelta;
		obj.anchoredPosition = rTResolutionPreset.anchPosition;
		obj.anchorMin = rTResolutionPreset.anchMin;
		obj.anchorMax = rTResolutionPreset.anchMax;
	}

	protected override void WritePreset(ResolutionType type)
	{
		RTResolutionPreset rTResolutionPreset = presets.First((RTResolutionPreset x) => x.type == type);
		RectTransform rectTransform = base.transform as RectTransform;
		rTResolutionPreset.anchPosition = rectTransform.anchoredPosition;
		rTResolutionPreset.sizeDelta = rectTransform.sizeDelta;
		rTResolutionPreset.pivot = rectTransform.pivot;
		rTResolutionPreset.anchMin = rectTransform.anchorMin;
		rTResolutionPreset.anchMax = rectTransform.anchorMax;
	}

	public void OverridePresets(List<RTResolutionPreset> newPresets)
	{
		presets = newPresets;
		Start();
	}
}
