using UnityEngine;

namespace Merge;

public static class Pocket
{
	public static Vector3 AnchorPosition => Controller<PocketController>.Instance.AnchorPosition;

	public static void AddItemToQueue(GIData item, PlayType playType = PlayType.story)
	{
		if (Controller<PocketController>.Instance != null)
		{
			Controller<PocketController>.Instance.AddItemToQueue(item, playType);
		}
	}

	public static void ClearEventData()
	{
		if (Controller<PocketController>.Instance != null)
		{
			Controller<PocketController>.Instance.ClearEventData();
		}
	}
}
