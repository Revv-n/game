using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MISC.Resolution;

public class LayoutGroupResolutionAdapter : ResolutionAdapter
{
	[Serializable]
	public class LayoutResolutionPreset : ResolutionPreset
	{
		public bool isHorisontasl;

		public RectOffset padding;

		public float spacing;

		public TextAnchor childAlignment;

		public bool childControlHeight;

		public bool childControlWidth;

		public bool childForceExpandHeight;

		public bool childForceExpandWidth;
	}

	[SerializeField]
	private List<LayoutResolutionPreset> presets = new List<LayoutResolutionPreset>(new LayoutResolutionPreset[3]
	{
		new LayoutResolutionPreset
		{
			type = ResolutionType.Wide
		},
		new LayoutResolutionPreset
		{
			type = ResolutionType.Old
		},
		new LayoutResolutionPreset
		{
			type = ResolutionType.IPad
		}
	});

	public override void Adaptate(ResolutionType type)
	{
		LayoutResolutionPreset layoutResolutionPreset = presets.First((LayoutResolutionPreset x) => x.type == type);
		HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
		bool flag = horizontalOrVerticalLayoutGroup is HorizontalLayoutGroup != layoutResolutionPreset.isHorisontasl;
		if (flag && Application.isPlaying)
		{
			StartCoroutine(CRT_ReplaceGroup(layoutResolutionPreset, horizontalOrVerticalLayoutGroup));
			return;
		}
		if (!Application.isPlaying && flag)
		{
			UnityEngine.Object.DestroyImmediate(horizontalOrVerticalLayoutGroup);
			horizontalOrVerticalLayoutGroup = null;
		}
		if (horizontalOrVerticalLayoutGroup == null)
		{
			horizontalOrVerticalLayoutGroup = AddCorrectGroup(layoutResolutionPreset);
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
		}
		AdaptateGroup(layoutResolutionPreset, horizontalOrVerticalLayoutGroup);
	}

	private HorizontalOrVerticalLayoutGroup AddCorrectGroup(LayoutResolutionPreset preset)
	{
		HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup = null;
		if (preset.isHorisontasl)
		{
			return base.gameObject.AddComponent<HorizontalLayoutGroup>();
		}
		return base.gameObject.AddComponent<VerticalLayoutGroup>();
	}

	private IEnumerator CRT_ReplaceGroup(LayoutResolutionPreset preset, HorizontalOrVerticalLayoutGroup layoutGroup)
	{
		UnityEngine.Object.Destroy(layoutGroup);
		yield return new WaitForEndOfFrame();
		HorizontalOrVerticalLayoutGroup layoutGroup2 = AddCorrectGroup(preset);
		AdaptateGroup(preset, layoutGroup2);
		LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
	}

	private void AdaptateGroup(LayoutResolutionPreset preset, HorizontalOrVerticalLayoutGroup layoutGroup)
	{
		layoutGroup.padding = preset.padding;
		layoutGroup.spacing = preset.spacing;
		layoutGroup.childAlignment = preset.childAlignment;
		layoutGroup.childControlHeight = preset.childControlHeight;
		layoutGroup.childControlWidth = preset.childControlWidth;
		layoutGroup.childForceExpandHeight = preset.childForceExpandHeight;
		layoutGroup.childForceExpandWidth = preset.childForceExpandWidth;
	}

	protected override void WritePreset(ResolutionType type)
	{
		LayoutResolutionPreset layoutResolutionPreset = presets.First((LayoutResolutionPreset x) => x.type == type);
		HorizontalOrVerticalLayoutGroup component = GetComponent<HorizontalOrVerticalLayoutGroup>();
		layoutResolutionPreset.isHorisontasl = component is HorizontalLayoutGroup;
		layoutResolutionPreset.padding = component.padding;
		layoutResolutionPreset.spacing = component.spacing;
		layoutResolutionPreset.childAlignment = component.childAlignment;
		layoutResolutionPreset.childControlHeight = component.childControlHeight;
		layoutResolutionPreset.childControlWidth = component.childControlWidth;
		layoutResolutionPreset.childForceExpandHeight = component.childForceExpandHeight;
		layoutResolutionPreset.childForceExpandWidth = component.childForceExpandWidth;
	}
}
