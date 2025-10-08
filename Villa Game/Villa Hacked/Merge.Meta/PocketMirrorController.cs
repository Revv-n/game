using Merge.Core.Masters;
using UnityEngine;

namespace Merge.Meta;

public class PocketMirrorController : Controller<PocketMirrorController>, IDataController
{
	[SerializeField]
	private Transform spawnAnchor;

	private PocketController.PocketData data;

	public Vector3 AnchorPosition => spawnAnchor.position;

	Data IDataController.GetSave()
	{
		return data;
	}

	void IDataController.Load(Data baseData)
	{
		data = (baseData as PocketController.PocketData) ?? new PocketController.PocketData();
		data.Fix_1_3_0();
	}

	public void AddItemToQueue(GIData item, PlayType playType = PlayType.story)
	{
		switch (playType)
		{
		case PlayType.story:
			data.PocketItemsQueue.Enqueue(item);
			break;
		case PlayType.events:
			data.PocketEventItemsQueue.Enqueue(item);
			break;
		}
	}

	public void ClearEventData()
	{
		data.PocketEventItemsQueue.Clear();
	}
}
