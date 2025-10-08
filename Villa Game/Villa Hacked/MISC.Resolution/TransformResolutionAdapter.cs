using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MISC.Resolution;

public class TransformResolutionAdapter : ResolutionAdapter
{
	[Serializable]
	public class TransResolutionPreset : ResolutionPreset
	{
		public Vector3 localPosition;

		public Vector3 localScale;
	}

	[SerializeField]
	private bool usePosition = true;

	[SerializeField]
	private bool useScale = true;

	[SerializeField]
	private List<TransResolutionPreset> presets = new List<TransResolutionPreset>(new TransResolutionPreset[3]
	{
		new TransResolutionPreset
		{
			type = ResolutionType.Wide
		},
		new TransResolutionPreset
		{
			type = ResolutionType.Old
		},
		new TransResolutionPreset
		{
			type = ResolutionType.IPad
		}
	});

	public override void Adaptate(ResolutionType type)
	{
		TransResolutionPreset transResolutionPreset = presets.First((TransResolutionPreset x) => x.type == type);
		Transform transform = base.transform;
		if (usePosition)
		{
			transform.localPosition = transResolutionPreset.localPosition;
		}
		if (useScale)
		{
			transform.localScale = transResolutionPreset.localScale;
		}
	}

	protected override void WritePreset(ResolutionType type)
	{
		TransResolutionPreset transResolutionPreset = presets.First((TransResolutionPreset x) => x.type == type);
		Transform transform = base.transform;
		transResolutionPreset.localPosition = transform.localPosition;
		transResolutionPreset.localScale = transform.localScale;
	}
}
