using GreenT.HornyScapes.Presents.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Presents.Managers;

public class PresentBundleManager : SimpleManager<PresentBundleData>
{
	public PresentBundleData GetBundle(int presentId)
	{
		foreach (PresentBundleData item in collection)
		{
			if (item.PresentId == presentId)
			{
				return item;
			}
		}
		return null;
	}
}
