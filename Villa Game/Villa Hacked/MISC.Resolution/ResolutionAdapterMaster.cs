using System.Collections.Generic;
using UnityEngine;

namespace MISC.Resolution;

public class ResolutionAdapterMaster : BaseController
{
	[SerializeField]
	private List<ResolutionAdapter> earlyInitAdapters;

	private static bool resolutionSelected = false;

	public override int PreloadOrder => Priority.VeryHigh;

	public static ResolutionType Resolution { get; private set; } = ResolutionType.Old;


	public override void Preload()
	{
		if (resolutionSelected)
		{
			EarlyInit();
			return;
		}
		Camera main = Camera.main;
		float num = (float)main.pixelWidth / (float)main.pixelHeight;
		if (num > 1.9f)
		{
			Resolution = ResolutionType.Wide;
		}
		else if (num > 1.65f && num <= 1.9f)
		{
			Resolution = ResolutionType.Old;
		}
		else
		{
			Resolution = ResolutionType.IPad;
		}
		resolutionSelected = true;
		EarlyInit();
	}

	private void EarlyInit()
	{
		foreach (ResolutionAdapter earlyInitAdapter in earlyInitAdapters)
		{
			earlyInitAdapter.Adaptate(Resolution);
			earlyInitAdapter.enabled = false;
		}
	}
}
