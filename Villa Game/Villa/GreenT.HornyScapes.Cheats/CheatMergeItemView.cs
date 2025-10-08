using System.Linq;
using System.Text;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.ToolTips;
using Merge;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeItemView : MergeItemCollectionView
{
	private InfoGetItem infoGetItem;

	private ModifyController modifyController;

	public void Set(InfoGetItem infoGetItem, ModifyController modifyController)
	{
		this.infoGetItem = infoGetItem;
		this.modifyController = modifyController;
	}

	public void PrintInfo()
	{
		if (infoGetItem.Spawners.ContainsKey(Source))
		{
			_ = infoGetItem.Spawners[Source];
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (infoGetItem.HowToGetDict.TryGetValue(Source.Key, out var value))
		{
			int[] array = value?.Select((GIConfig _item) => _item.UniqId).ToArray();
			for (int i = 0; i < array.Length - 1; i++)
			{
				stringBuilder.Append(array[i] + ", ");
			}
			stringBuilder.Append(array.Last());
		}
	}

	public void AddItemToPocket()
	{
		Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.AddItemToQueue(Source.Key);
	}
}
