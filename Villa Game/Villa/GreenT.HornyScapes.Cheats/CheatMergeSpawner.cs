using System.Linq;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Cheats;

public class CheatMergeSpawner : CheateMerge
{
	private void Update()
	{
		if (inputSetting.DischargeAnySpawner.IsPressedKeys)
		{
			DischargeAnySpawner();
		}
		if (inputSetting.IsPressed(inputSetting.DischargeSelectedSpawner))
		{
			DischargeSelectedSpawner();
		}
	}

	[EditorButton]
	[ContextMenu("Discharge selected spawner")]
	public void DischargeSelectedSpawner()
	{
		GameItem selected = base.selectionController.Selected;
		if (!base.selectionController.Selected.IsEmptySpawner)
		{
			SpawnUntilFullBySpawner(selected);
		}
	}

	[EditorButton]
	[ContextMenu("Discharge any spawner")]
	public void DischargeAnySpawner()
	{
		if (FindAnySpawner(out var availableSpawner))
		{
			SpawnUntilFullBySpawner(availableSpawner);
		}
	}

	private void SpawnUntilFullAnySpawner()
	{
		if (FindAnySpawner(out var availableSpawner))
		{
			SpawnUntilFullBySpawner(availableSpawner);
		}
	}

	private void SpawnUntilFullBySpawner(GameItem gameItem)
	{
		GIBox.ClickSpawn box = gameItem.GetBox<GIBox.ClickSpawn>();
		int num = 0;
		while (box.CanSpawn && !base.itemController.CurrentField.IsFull && num < 100)
		{
			InteractSpawner(gameItem);
			num++;
		}
	}

	private bool FindAnySpawner(out GameItem availableSpawner)
	{
		availableSpawner = base.itemController.CurrentField.Field.Objects.FirstOrDefault((GameItem _item) => !_item.IsEmptySpawner && _item.AllowInteraction(GIModuleType.ClickSpawn));
		_ = availableSpawner == null;
		return availableSpawner != null;
	}

	private void InteractSpawner(GameItem availableSpawner)
	{
		base.clickSpawnController.Interact(availableSpawner);
	}
}
