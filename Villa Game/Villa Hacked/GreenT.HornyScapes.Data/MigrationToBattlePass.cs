using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.MergeCore;
using Merge;

namespace GreenT.HornyScapes.Data;

public static class MigrationToBattlePass
{
	private const bool UseMigrate = true;

	public static void Migrate(List<GIData> dataCase)
	{
		bool flag = false;
		int num = 0;
		HashSet<GIData> migrated = new HashSet<GIData>();
		foreach (GIData item in dataCase)
		{
			if (!ValidateData(item))
			{
				migrated.Add(item);
				num += item.Key.ID;
				flag = true;
			}
		}
		if (flag)
		{
			dataCase.RemoveAll((GIData el) => migrated.Contains(el));
		}
	}

	public static void Migrate(GreenT.HornyScapes.MergeCore.PocketController.PocketMemento pocketMemento)
	{
		bool flag = false;
		flag = Migrate(pocketMemento.PocketEventItemsQueue) || flag;
		flag = Migrate(pocketMemento.PocketItemsQueue) || flag;
	}

	public static bool Migrate(Queue<GIData> queue)
	{
		if (queue == null)
		{
			return false;
		}
		int num = 0;
		GIData[] array = queue.Where(ValidateData).ToArray();
		if (array.Length != queue.Count)
		{
			num = queue.Where((GIData giData) => !ValidateData(giData)).Sum((GIData giData) => giData.Key.ID);
		}
		if (num > 0)
		{
			queue.Clear();
			GIData[] array2 = array;
			foreach (GIData item in array2)
			{
				queue.Enqueue(item);
			}
		}
		return true;
	}

	public static void Migrate(List<InventoryController.GeneralData.ItemNode> dataStoredItems)
	{
		bool flag = false;
		int num = 0;
		HashSet<InventoryController.GeneralData.ItemNode> migrated = new HashSet<InventoryController.GeneralData.ItemNode>();
		foreach (InventoryController.GeneralData.ItemNode dataStoredItem in dataStoredItems)
		{
			if (!ValidateData(dataStoredItem.Item))
			{
				migrated.Add(dataStoredItem);
				num += dataStoredItem.Item.Key.ID;
				flag = true;
			}
		}
		if (flag)
		{
			dataStoredItems.RemoveAll((InventoryController.GeneralData.ItemNode el) => migrated.Contains(el));
		}
	}

	private static bool ValidateData(GIData giData)
	{
		if (giData != null)
		{
			_ = giData.Key;
			if (0 == 0)
			{
				return !giData.Key.Collection.Equals("XpStar");
			}
		}
		return false;
	}
}
