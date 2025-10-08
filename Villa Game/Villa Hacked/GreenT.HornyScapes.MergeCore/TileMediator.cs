using System;
using System.Collections.Generic;
using System.Linq;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class TileMediator : Controller<TileMediator>
{
	private Dictionary<IControlClocks, Action<IControlClocks>> actionsDict = new Dictionary<IControlClocks, Action<IControlClocks>>();

	private GameItemController gameItemController => Controller<GameItemController>.Instance;

	private bool IsLocked(GameItem item)
	{
		if (!item.IsLocked)
		{
			return item.IsBubbled;
		}
		return true;
	}

	private bool IsLockedAndEmpty(GameItem item)
	{
		if (!IsLocked(item))
		{
			return item.IsEmptySpawner;
		}
		return true;
	}

	public override void Init()
	{
		base.Init();
		gameItemController.OnItemCreated -= ExtraSpawnSubscribe;
		gameItemController.OnItemCreated += ExtraSpawnSubscribe;
		gameItemController.OnItemTakenFromSomethere -= ExtraSpawnSubscribe;
		gameItemController.OnItemTakenFromSomethere += ExtraSpawnSubscribe;
		foreach (GameItem @object in gameItemController.CurrentField.Field.Objects)
		{
			ExtraSpawnSubscribe(@object);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Controller<GameItemController>.Instance != null)
		{
			Controller<GameItemController>.Instance.OnItemCreated -= ExtraSpawnSubscribe;
			Controller<GameItemController>.Instance.OnItemTakenFromSomethere -= ExtraSpawnSubscribe;
		}
	}

	public void ExtraTaskSubscribe(GameItem item)
	{
		item.ChangeExtraMarkRef(1);
		item.OnMovedFrom += ExtraTaskSubOnEndDrag;
		item.OnRemoving += ExtraTaskUnsubscribe;
		item.OnBecomeDrag += ExtraSubOnDrag;
		SubscribeOnBlockChanged(item);
		SetTaskViewSubTile(item.Coordinates);
		RefreshTaskTile(item);
	}

	public void ExtraTaskUnsubscribe(GameItem item)
	{
		item.ChangeExtraMarkRef(-1);
		item.OnRemoving -= ExtraTaskUnsubscribe;
		item.OnBecomeDrag -= ExtraSubOnDrag;
		item.OnMovedFrom -= ExtraTaskSubOnEndDrag;
		UnsubscribeOnBlockChanged(item);
		if (item.ExtraMarkRef <= 0)
		{
			DeactivateSubTile(item);
		}
	}

	public void ExtraSpawnSubscribe(GameItem item)
	{
		if (item.Data.HasModule(GIModuleType.ClickSpawn))
		{
			item.OnMovedFrom += ExtraSpawnSubOnEndDrag;
			item.OnRemoving += ExtraSpawnUnsubscribe;
			item.OnBecomeDrag += ExtraSubOnDrag;
			item.OnTimerActiveChange += SetSubTileByTimer;
			SubscribeOnBlockChanged(item);
			SetSpawnViewSubTile(item.Coordinates);
			RefreshSpawnerTile(item);
		}
	}

	public void ExtraSpawnUnsubscribe(GameItem item)
	{
		item.OnRemoving -= ExtraSpawnUnsubscribe;
		item.OnBecomeDrag -= ExtraSubOnDrag;
		item.OnMovedFrom -= ExtraSpawnSubOnEndDrag;
		item.OnTimerActiveChange -= SetSubTileByTimer;
		UnsubscribeOnBlockChanged(item);
		DeactivateSubTile(item);
	}

	private void ExtraSubOnDrag(GameItem item)
	{
		DeactivateSubTile(item);
	}

	private void ExtraTaskSubOnEndDrag(GameItem item, Point oldCoord)
	{
		SetTaskViewSubTile(item.Coordinates);
		if (IsLocked(item))
		{
			SubscribeOnBlockChanged(item);
		}
		else
		{
			RefreshShiftedTile(item, oldCoord);
		}
	}

	private void ExtraSpawnSubOnEndDrag(GameItem item, Point oldCoord)
	{
		SetSpawnViewSubTile(item.Coordinates);
		if (item.IsEmptySpawner)
		{
			SubscribeOnTimerComplete(item, oldCoord);
		}
		if (IsLockedAndEmpty(item))
		{
			SubscribeOnBlockChanged(item);
		}
		else
		{
			RefreshShiftedTile(item, oldCoord);
		}
	}

	private void RefreshTaskTile(GameItem item)
	{
		if (IsLocked(item))
		{
			DeactivateSubTile(item);
		}
		else
		{
			ActivateSubTile(item);
		}
	}

	private void RefreshSpawnerTile(GameItem item)
	{
		if (IsLockedAndEmpty(item))
		{
			DeactivateSubTile(item);
		}
		else
		{
			ActivateSubTile(item);
		}
	}

	private void RefreshSpawnerTileCrunch(GameItem item)
	{
		if (IsLockedAndEmpty(item))
		{
			DeactivateSubTile(item);
		}
		else
		{
			ActivateSubTileCrunch(item);
		}
	}

	private void RefreshShiftedTile(GameItem item, Point oldCoord)
	{
		GameItemController instance = Controller<GameItemController>.Instance;
		if (!instance.IsPointBusy(oldCoord))
		{
			DeactivateSubTile(oldCoord);
		}
		else if (!instance.CurrentField.Field[oldCoord].HasLightSubTile)
		{
			DeactivateSubTile(oldCoord);
		}
		if (!item.IsRemovingNow)
		{
			ActivateSubTile(item);
		}
	}

	private void AtModuleDelete(GameItem item)
	{
		item.OnBlockActionChange -= AtModuleDelete;
		if (!item.IsRemovingNow)
		{
			RefreshShiftedTile(item, item.Coordinates);
		}
		else
		{
			DeactivateSubTile(item);
		}
	}

	private void SubscribeOnTimerComplete(GameItem item, Point oldCoord)
	{
		foreach (IControlClocks item2 in item.Boxes.OfType<IControlClocks>())
		{
			Action<IControlClocks> value = delegate(IControlClocks _control)
			{
				_control.OnTimerComplete -= actionsDict[_control];
				RefreshShiftedTile(item, oldCoord);
			};
			item2.OnTimerComplete += value;
			actionsDict[item2] = value;
		}
	}

	private void SubscribeOnBlockChanged(GameItem item)
	{
		item.OnBlockActionChange -= AtModuleDelete;
		item.OnBlockActionChange += AtModuleDelete;
	}

	private void UnsubscribeOnBlockChanged(GameItem item)
	{
		item.OnBlockActionChange -= AtModuleDelete;
	}

	private void SetSpawnViewSubTile(Point tile)
	{
		Controller<SubTileSystem>.Instance.SetLight(tile, isLock: true);
	}

	private void SetTaskViewSubTile(Point point)
	{
		Controller<SubTileSystem>.Instance.SetLight(point, isLock: false);
	}

	private void DeactivateSubTile(Point oldCoord)
	{
		Controller<SubTileSystem>.Instance.Hide(oldCoord);
	}

	private void DeactivateSubTile(GameItem item)
	{
		item.HasLightSubTile = false;
		Controller<SubTileSystem>.Instance.Hide(item.Coordinates);
	}

	private void SetSubTileByTimer(GameItem item, bool timerVisible)
	{
		item.HasLightSubTile = timerVisible;
		if (timerVisible)
		{
			Controller<SubTileSystem>.Instance.Hide(item);
		}
		else
		{
			RefreshSpawnerTileCrunch(item);
		}
	}

	private void ActivateSubTile(GameItem item)
	{
		item.HasLightSubTile = true;
		Controller<SubTileSystem>.Instance.Show(item.Coordinates);
	}

	private void ActivateSubTileCrunch(GameItem item)
	{
		item.HasLightSubTile = true;
		Controller<SubTileSystem>.Instance.Show(item);
	}
}
