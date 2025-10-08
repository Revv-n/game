using System.Collections.Generic;
using System.Linq;
using Merge;
using Merge.Core.Masters;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class TeslaController : Controller<TeslaController>, ICreateItemListener, IActionModuleController, IModuleController
{
	public const string SPEED_UP_KEY = "Tesla";

	[SerializeField]
	private GameObject teslaEffect;

	[Inject]
	private MergeFieldProvider mergeFieldProvider;

	int ICreateItemListener.Priority => Priority.Low;

	GIModuleType IModuleController.ModuleType => GIModuleType.Tesla;

	public override void Init()
	{
		base.Init();
		MotionController.OnItemBecomesDrag += AtBecomesDrag;
		MotionController.OnItemMovedFrom += AtItemPlaced;
	}

	protected override void OnDestroy()
	{
		MotionController.OnItemBecomesDrag -= AtBecomesDrag;
		MotionController.OnItemMovedFrom -= AtItemPlaced;
		base.OnDestroy();
	}

	public GameObject CreateTelaEffect()
	{
		return Object.Instantiate(teslaEffect);
	}

	void ICreateItemListener.AtItemCreated(GameItem gi, MergeField mergeField)
	{
		if (!gi.Config.TryGetModule<ModuleConfigs.Tesla>(out var result))
		{
			AtItemPlaced(gi, gi.Coordinates);
			return;
		}
		ModuleDatas.Tesla tesla = gi.Data.GetModule<ModuleDatas.Tesla>();
		if (tesla == null)
		{
			tesla = new ModuleDatas.Tesla();
			gi.Data.Modules.Add(tesla);
		}
		GIBox.Tesla tesla2 = new GIBox.Tesla(tesla, result);
		tesla2.OnTimerComplete += AtBoxTimerComplete;
		gi.AddBox(tesla2);
		if (tesla.Activated)
		{
			if (tesla.LifeTimer.IsCompleted)
			{
				AtBoxTimerComplete(tesla2);
				return;
			}
			tesla2.StartTimer();
			AtItemPlaced(gi, gi.Coordinates);
		}
	}

	private void AtBecomesDrag(GameItem dragging)
	{
		if (dragging.TryGetBox<GIBox.Tesla>(out var box))
		{
			List<Point> tilesDonut = GIMaster.Field.GetTilesDonut(dragging.Coordinates, (GameItem x) => x != null && x.Boxes.Any((GIBox.Base b) => b is ISpeedUpReciver));
			if (tilesDonut.Count == 0)
			{
				return;
			}
			{
				foreach (GIBox.Base item in from x in tilesDonut.SelectMany((Point x) => GIMaster.Field.CurrentField.Field[x].Boxes)
					where x is ISpeedUpReciver
					select x)
				{
					(item as ISpeedUpReciver).RemoveSpeedUpSource(box);
				}
				return;
			}
		}
		IEnumerable<GIBox.Base> enumerable = dragging.Boxes.Where((GIBox.Base x) => x is ISpeedUpReciver);
		if (enumerable.Count() == 0)
		{
			return;
		}
		foreach (GIBox.Base item2 in enumerable)
		{
			(item2 as ISpeedUpReciver).RemoveSpeedUpSourcesWithKey("Tesla");
		}
	}

	private void AtItemPlaced(GameItem placed, Point startPos)
	{
		if (placed.TryGetBox<GIBox.Tesla>(out var box))
		{
			if (!box.Data.Activated)
			{
				return;
			}
			{
				foreach (GIBox.Base item in from x in GIMaster.Field.GetTilesDonut(placed.Coordinates, (GameItem x) => x != null && x.Boxes.Any((GIBox.Base b) => b is ISpeedUpReciver)).SelectMany((Point x) => GIMaster.Field.CurrentField.Field[x].Boxes)
					where x is ISpeedUpReciver
					select x)
				{
					(item as ISpeedUpReciver).AddSpeedUpSource(box);
				}
				return;
			}
		}
		IEnumerable<GIBox.Base> enumerable = placed.Boxes.Where((GIBox.Base x) => x is ISpeedUpReciver);
		if (enumerable.Count() == 0)
		{
			return;
		}
		GIBox.Tesla box2;
		List<Point> tilesDonut = GIMaster.Field.GetTilesDonut(placed.Coordinates, (GameItem x) => x != null && x.TryGetBox<GIBox.Tesla>(out box2));
		if (tilesDonut.Count == 0)
		{
			return;
		}
		IEnumerable<GIBox.Tesla> enumerable2 = from x in tilesDonut.SelectMany((Point x) => GIMaster.Field.CurrentField.Field[x].Boxes)
			where x is GIBox.Tesla
			select x as GIBox.Tesla into x
			where x.Data.Activated
			select x;
		foreach (GIBox.Base item2 in enumerable)
		{
			foreach (GIBox.Tesla item3 in enumerable2)
			{
				(item2 as ISpeedUpReciver).AddSpeedUpSource(item3);
			}
		}
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		(box as GIBox.Tesla).Activate();
		AtItemPlaced(box.Parent, box.Parent.Coordinates);
	}

	private void AtBoxTimerComplete(IControlClocks sender)
	{
		GIBox.Tesla tesla = sender as GIBox.Tesla;
		foreach (GIBox.Base item in from x in GIMaster.Field.GetTilesDonut(tesla.Parent.Coordinates, (GameItem x) => x != null && x.Boxes.Any((GIBox.Base b) => b is ISpeedUpReciver)).SelectMany((Point x) => GIMaster.Field.CurrentField.Field[x].Boxes)
			where x is ISpeedUpReciver
			select x)
		{
			(item as ISpeedUpReciver).RemoveSpeedUpSource(tesla);
		}
		if (tesla.Config.DestroyType == GIDestroyType.Transform)
		{
			if (mergeFieldProvider.TryGetFieldWithItem(tesla.Parent, out var data))
			{
				GIMaster.SwapItem(data, tesla.Parent, tesla.Config.DestroyResult.Copy().SetCoordinates(tesla.Parent.Coordinates));
			}
		}
		else if (tesla.Config.DestroyType == GIDestroyType.Destroy)
		{
			GIMaster.Field.RemoveItem(tesla.Parent);
		}
	}
}
