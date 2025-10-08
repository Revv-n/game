using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MISC.Resolution;

public class ImageResoulutionAdapter : ResolutionAdapter
{
	[Serializable]
	public class ImageResolutionPreset : ResolutionPreset
	{
		public float pixelsPerUnitMul = 1f;

		public string overrideSprite;
	}

	[SerializeField]
	private List<ImageResolutionPreset> presets = new List<ImageResolutionPreset>(new ImageResolutionPreset[3]
	{
		new ImageResolutionPreset
		{
			type = ResolutionType.Wide
		},
		new ImageResolutionPreset
		{
			type = ResolutionType.Old
		},
		new ImageResolutionPreset
		{
			type = ResolutionType.IPad
		}
	});

	public override void Adaptate(ResolutionType type)
	{
		ImageResolutionPreset imageResolutionPreset = presets.First((ImageResolutionPreset x) => x.type == type);
		Image component = GetComponent<Image>();
		component.pixelsPerUnitMultiplier = imageResolutionPreset.pixelsPerUnitMul;
		if (!string.IsNullOrEmpty(imageResolutionPreset.overrideSprite))
		{
			component.sprite = Resources.Load<Sprite>(imageResolutionPreset.overrideSprite);
		}
	}

	protected override void WritePreset(ResolutionType type)
	{
		ImageResolutionPreset imageResolutionPreset = presets.First((ImageResolutionPreset x) => x.type == type);
		Image component = GetComponent<Image>();
		imageResolutionPreset.pixelsPerUnitMul = component.pixelsPerUnitMultiplier;
	}
}
