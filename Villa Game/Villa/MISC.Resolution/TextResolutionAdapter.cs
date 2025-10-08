using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MISC.Resolution;

public class TextResolutionAdapter : ResolutionAdapter
{
	[Serializable]
	public class TextResolutionPreset : ResolutionPreset
	{
		public bool resizeTextForBestFit;

		public int fontSize;

		public int resizeTextMaxSize;

		public int resizeTextMinSize;

		public TextAnchor alignment;
	}

	[SerializeField]
	private List<TextResolutionPreset> presets = new List<TextResolutionPreset>(new TextResolutionPreset[3]
	{
		new TextResolutionPreset
		{
			type = ResolutionType.Wide
		},
		new TextResolutionPreset
		{
			type = ResolutionType.Old
		},
		new TextResolutionPreset
		{
			type = ResolutionType.IPad
		}
	});

	public override void Adaptate(ResolutionType type)
	{
		TextResolutionPreset textResolutionPreset = presets.First((TextResolutionPreset x) => x.type == type);
		UniversalText.SetAllowBestFit(base.gameObject, textResolutionPreset.resizeTextForBestFit);
		UniversalText.SetFontSize(base.gameObject, textResolutionPreset.fontSize);
		UniversalText.SetMaxAutoSize(base.gameObject, textResolutionPreset.resizeTextMaxSize);
		UniversalText.SetMinAutoSize(base.gameObject, textResolutionPreset.resizeTextMinSize);
		UniversalText.SetAligment(base.gameObject, textResolutionPreset.alignment);
	}

	protected override void WritePreset(ResolutionType type)
	{
		TextResolutionPreset textResolutionPreset = presets.First((TextResolutionPreset x) => x.type == type);
		GetComponent<Text>();
		textResolutionPreset.resizeTextForBestFit = UniversalText.GetAllowBestFit(base.gameObject);
		textResolutionPreset.fontSize = (int)UniversalText.GetFontSize(base.gameObject);
		textResolutionPreset.resizeTextMaxSize = (int)UniversalText.GetMaxAutoSize(base.gameObject);
		textResolutionPreset.resizeTextMinSize = (int)UniversalText.GetMinAutoSize(base.gameObject);
		textResolutionPreset.alignment = UniversalText.GetAligment(base.gameObject);
	}
}
