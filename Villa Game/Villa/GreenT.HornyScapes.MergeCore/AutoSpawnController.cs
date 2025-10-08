using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class AutoSpawnController : Controller<AutoSpawnController>, ITileBecomesEmptyListener, IActionModuleController, IModuleController, ICreateItemListener, ITimeBoostListener
{
	[SerializeField]
	private LightningTweenBuilder lightningPrefab;

	[Space]
	[Header("Animation")]
	[SerializeField]
	private float radius;

	[SerializeField]
	private int amountDrig;

	[SerializeField]
	private float timeDrig;

	[SerializeField]
	private float deltaPositionY;

	[SerializeField]
	private float endSize;

	[SerializeField]
	private CurrencyType priceType = CurrencyType.Hard;

	private ICurrencyProcessor currencyProcessor;

	private ModifyController modifyController;

	private MergeFieldProvider mergeFieldProvider;

	private HashSet<GIBox.AutoSpawn> emptyTileWaiters = new HashSet<GIBox.AutoSpawn>();

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.AutoSpawn;

	int ICreateItemListener.Priority => Priority.Normal;

	[Inject]
	public void Init(ICurrencyProcessor currencyProcessor, ModifyController modifyController, MergeFieldProvider mergeFieldProvider)
	{
		this.currencyProcessor = currencyProcessor;
		this.modifyController = modifyController;
		this.mergeFieldProvider = mergeFieldProvider;
	}

	public override void Preload()
	{
		base.Preload();
		Field.OnItemRemoved += AtItemRemoved;
		MotionController.OnItemMovedFrom += AtItemMoved;
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		ModuleConfigs.AutoSpawn result;
		if (item.Data.HasModule<ModuleDatas.AutoSpawn>() && !item.Config.HasModule<ModuleConfigs.AutoSpawn>())
		{
			ModuleDatas.AutoSpawn module = item.Data.GetModule<ModuleDatas.AutoSpawn>();
			item.Data.Modules.Remove(module);
		}
		else if (item.Config.TryGetModule<ModuleConfigs.AutoSpawn>(out result))
		{
			GIBox.AutoSpawn autoSpawn = new GIBox.AutoSpawn(item.Data.GetModule<ModuleDatas.AutoSpawn>() ?? AddDefaultModuleData(item, result), modifiedMaxAmount: modifyController.CalcMaxAmount(result.MaxAmount, item), mConfig: result);
			autoSpawn.OnTimerComplete += AtBoxTimerComplite;
			item.SetIconClock();
			item.AddBox(autoSpawn);
			autoSpawn.AttachTweener(Object.Instantiate(lightningPrefab));
			if (item.AllowInteraction(GIModuleType.AutoSpawn))
			{
				TryProduce(autoSpawn);
			}
			else
			{
				item.OnBlockInteractionChange += AtItemInteractionChange;
			}
		}
	}

	private void AtItemRemoved(GameItem item)
	{
		GIBox.AutoSpawn box = item.GetBox<GIBox.AutoSpawn>();
		if (box != null)
		{
			emptyTileWaiters.Remove(box);
			box.OnTimerComplete -= AtBoxTimerComplite;
		}
	}

	private void AtBoxTimerComplite(IControlClocks sender)
	{
		GIBox.AutoSpawn autoSpawn = sender as GIBox.AutoSpawn;
		SetAmount(autoSpawn, autoSpawn.Config.RestoreAmount);
		TryProduce(autoSpawn);
		if (!autoSpawn.IsMaxAmount(modifyController.CalcMaxAmount(autoSpawn)))
		{
			autoSpawn.StartTimer(modifyController.RestoreTime(autoSpawn));
		}
	}

	private void SetAmount(GIBox.AutoSpawn box, int amount)
	{
		int num = Mathf.Min(amount, modifyController.CalcMaxAmount(box));
		box.Amount += num;
	}

	private bool TryProduce(GIBox.AutoSpawn box)
	{
		if (box.Data.Amount == 0)
		{
			return false;
		}
		if (!mergeFieldProvider.TryGetFieldWithItem(box.Parent, out var data))
		{
			return false;
		}
		List<Point> emptyTilesDonut = data.GetEmptyTilesDonut(box.Parent.Coordinates);
		if (emptyTilesDonut.Count == 0)
		{
			emptyTileWaiters.Add(box);
			return false;
		}
		List<WeightNode<GIData>> list = modifyController.RefreshModifySpawnPool(box);
		List<GIData> list2 = new List<GIData>();
		for (int i = 0; i < Mathf.Min(box.Data.Amount, emptyTilesDonut.Count); i++)
		{
			list2.Add(Weights.GetWeightObject((IList<WeightNode<GIData>>)list).Copy().SetCoordinates(emptyTilesDonut[i]));
		}
		SetAmount(box, -list2.Count);
		box.ValidateEffect();
		if (box.Amount == 0)
		{
			emptyTileWaiters.Remove(box);
		}
		else
		{
			emptyTileWaiters.Add(box);
		}
		if (!box.Data.TimerActive)
		{
			box.StartTimer(modifyController.RestoreTime(box));
		}
		foreach (GIData item in list2)
		{
			Field.CreateItem(item, data).DoCreateFrom(box.Parent.Position);
		}
		Merge.Sounds.Play("Spawn_in_time");
		CheckDestruction(box);
		return true;
	}

	private void CheckDestruction(GIBox.AutoSpawn box)
	{
		if (box.Data.Amount == 0 && box.Config.DestroyType != 0)
		{
			Field.RemoveItem(box.Parent);
			if (box.Config.DestroyType == GIDestroyType.Transform)
			{
				Field.CreateItem(box.Config.DestroyResult.Copy().SetCoordinates(box.Parent.Coordinates)).DoCreate();
			}
		}
	}

	private void AtItemInteractionChange(GameItem gi)
	{
		if (gi.AllowInteraction(GIModuleType.AutoSpawn))
		{
			gi.OnBlockInteractionChange -= AtItemInteractionChange;
			GIBox.AutoSpawn box = gi.GetBox<GIBox.AutoSpawn>();
			TryProduce(box);
		}
	}

	void ITimeBoostListener.AtTimeBoost(float value)
	{
		List<GIBox.AutoSpawn> list = (from x in Field.CurrentField.Field
			where x != null && x.Data.HasModule(GIModuleType.AutoSpawn)
			select x.GetBox<GIBox.AutoSpawn>()).ToList();
		IEnumerable<GIBox.AutoSpawn> enumerable = list.Where((GIBox.AutoSpawn x) => x.Data.TimerActive);
		Debug.Log($"Spawners found: {list.Count}, With active timer: {enumerable.Count()}");
		foreach (GIBox.AutoSpawn item in enumerable)
		{
			RefTimer refTimer = new RefTimer(item.TweenTimer.Timer.TotalTime, item.TweenTimer.Timer.StartTime.AddSeconds(0f - value));
			int num = refTimer.RemovePeriods();
			for (int i = 0; i < num; i++)
			{
				SetAmount(item, item.Config.RestoreAmount);
				TryProduce(item);
				if (item.Parent.IsRemovingNow)
				{
					break;
				}
			}
			int num2 = modifyController.CalcMaxAmount(item);
			item.Data.TimerActive = item.Data.Amount < num2;
			if (item.Data.TimerActive)
			{
				Debug.Log($"{item.Parent.Key} in {item.Parent.Coordinates} charges {item.Data.Amount}/{num2} time left {item.TweenTimer.Timer.TimeLeft}");
				item.StartTweenTimer(refTimer);
			}
			else
			{
				item.StopTimer();
				Debug.Log($"{item.Parent.Key} in {item.Parent.Coordinates} charges {item.Data.Amount}/{num2}");
			}
		}
	}

	void ITimeBoostListener.AtPlayAnim()
	{
		List<GIBox.AutoSpawn> source = (from x in Field.CurrentField.Field
			where x != null && x.Data.HasModule(GIModuleType.AutoSpawn)
			select x.GetBox<GIBox.AutoSpawn>()).ToList();
		List<GIBox.AutoSpawn> withActiveTimer = source.Where((GIBox.AutoSpawn x) => x.Data.TimerActive).ToList();
		Sequence sequence = DOTween.Sequence();
		List<Vector3> listPosition = new List<Vector3>();
		foreach (GIBox.AutoSpawn item in withActiveTimer)
		{
			Sequence sequence2 = DOTween.Sequence();
			listPosition.Add(item.Parent.transform.localPosition);
			for (int i = 0; i < amountDrig; i++)
			{
				Transform transform = item.Parent.transform;
				float y = Random.Range(transform.localPosition.y, transform.localPosition.y + radius * deltaPositionY);
				float x2 = Random.Range(transform.localPosition.x - radius, transform.localPosition.x + radius);
				sequence2.Append(item.Parent.transform.DOLocalMove(new Vector3(x2, y, transform.localPosition.z), timeDrig));
			}
			sequence.Join(item.Parent.transform.DOScale(endSize, timeDrig * (float)amountDrig));
			sequence.Join(sequence2);
			item.Parent.AppendOuterTween(sequence);
		}
		sequence.OnComplete(delegate
		{
			for (int j = 0; j < withActiveTimer.Count; j++)
			{
				withActiveTimer[j].Parent.transform.DOScale(1f, 0.3f).SetEase(Ease.InBack);
				withActiveTimer[j].Parent.transform.DOLocalMove(listPosition[j], 0.3f).SetEase(Ease.InBack);
			}
		});
	}

	void ITileBecomesEmptyListener.AtTileBecomesEmpty(Point emptyPoint)
	{
		GIBox.AutoSpawn autoSpawn = emptyTileWaiters.FirstOrDefault((GIBox.AutoSpawn x) => x.Parent.Coordinates.Distance(emptyPoint) == 1);
		if (autoSpawn != null)
		{
			TryProduce(autoSpawn);
			if (!autoSpawn.Data.TimerActive)
			{
				autoSpawn.StartTimer(modifyController.RestoreTime(autoSpawn));
			}
		}
	}

	private void AtItemMoved(GameItem item, Point startPoint)
	{
		GIBox.AutoSpawn autoSpawn = emptyTileWaiters.FirstOrDefault((GIBox.AutoSpawn x) => x.Parent == item);
		if (autoSpawn != null)
		{
			TryProduce(autoSpawn);
			if (!autoSpawn.Data.TimerActive)
			{
				autoSpawn.StartTimer(modifyController.RestoreTime(autoSpawn));
			}
		}
	}

	private ModuleDatas.AutoSpawn AddDefaultModuleData(GameItem sender, ModuleConfigs.AutoSpawn config)
	{
		ModuleDatas.AutoSpawn autoSpawn = new ModuleDatas.AutoSpawn();
		if (config.CanRestore)
		{
			autoSpawn.Amount = 0;
			autoSpawn.TimerActive = true;
			autoSpawn.MainTimer = new RefSkipableTimer(modifyController.RestoreTime(config.RestoreTime, sender));
		}
		else
		{
			autoSpawn.Amount = modifyController.CalcMaxAmount(config.MaxAmount, sender);
		}
		sender.Data.Modules.Add(autoSpawn);
		return autoSpawn;
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		GIBox.AutoSpawn autoSpawnBox = box as GIBox.AutoSpawn;
		if (currencyProcessor.TrySpent(priceType, autoSpawnBox.SpeedUpPrice))
		{
			BuyCallback();
		}
		void BuyCallback()
		{
			autoSpawnBox.StopTimer();
			AtBoxTimerComplite(autoSpawnBox);
		}
	}
}
