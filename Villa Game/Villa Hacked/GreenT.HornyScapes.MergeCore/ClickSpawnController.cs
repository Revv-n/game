using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.MergeStore;
using GreenT.Types;
using GreenT.UI;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class ClickSpawnController : Controller<ClickSpawnController>, IClickInteractionController, IModuleController, IActionModuleController, ICreateItemListener, ITimeBoostListener
{
	[SerializeField]
	private LightningTweenBuilder lightningPrefab;

	[SerializeField]
	private DestructionTweenBuilder destructionTweenBuilder;

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
	private SpeedUpStrategy _speedUpClickSpawnStrategy;

	private float firstRoomBoost = 0.02222f;

	private ICurrencyProcessor currencyProcessor;

	private ModifyController modifyController;

	private IWindowsManager windowsManager;

	private RestoreEnergyPopupOpener restoreEnergyPopupOpener;

	private RestoreEventEnergyPopupOpener restoreEventEnergyPopupOpener;

	private SignalBus signalBus;

	private int tabIDNoHards;

	private BankWindow bankWindow;

	private SpawnerReloader spawnerReloader;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	private EventEnergyModeTempService eventEnergyModeTempService;

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.ClickSpawn;

	int ICreateItemListener.Priority => Priority.Normal;

	public event Action<GameItem, GameItem> OnClickSpawn;

	public event Action<GIBox.ClickSpawn> OnRefreshSpawner;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, ICurrencyProcessor currencyProcessor, ModifyController modifyController, SignalBus signalBus, IConstants<int> intConstants, SpawnerReloader spawnerReloader, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic, RestoreEnergyPopupOpener restoreEnergyPopupOpener, EventEnergyModeTempService eventEnergyModeTempService, RestoreEventEnergyPopupOpener restoreEventEnergyPopupOpener)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.windowsManager = windowsManager;
		this.currencyProcessor = currencyProcessor;
		this.modifyController = modifyController;
		tabIDNoHards = intConstants["banktab_no_hards"];
		this.signalBus = signalBus;
		this.spawnerReloader = spawnerReloader;
		this.restoreEnergyPopupOpener = restoreEnergyPopupOpener;
		this.restoreEventEnergyPopupOpener = restoreEventEnergyPopupOpener;
		this.eventEnergyModeTempService = eventEnergyModeTempService;
	}

	public override void Preload()
	{
		base.Preload();
		Field.AfterItemCreated += AfterItemCreated;
	}

	public override void Init()
	{
		spawnerReloader.Initialize();
		Controller<LockedController>.Instance.OnItemActionUnlock += AtItemUnlock;
		bankWindow = windowsManager.Get<BankWindow>();
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (item.Config.TryGetModule<ModuleConfigs.ClickSpawn>(out var result))
		{
			ModuleDatas.ClickSpawn clickSpawn = item.Data.GetModule<ModuleDatas.ClickSpawn>();
			int num = modifyController.CalcMaxAmount(result.MaxAmount, item);
			if (clickSpawn == null)
			{
				clickSpawn = new ModuleDatas.ClickSpawn();
				clickSpawn.Amount = num;
				item.Data.Modules.Add(clickSpawn);
			}
			GIBox.ClickSpawn clickSpawn2 = new GIBox.ClickSpawn(clickSpawn, result, num);
			clickSpawn2.OnTimerComplete += AtBoxTimerComplete;
			item.SetIconClock();
			item.AddBox(clickSpawn2);
		}
	}

	private void AfterItemCreated(GameItem item)
	{
		if (item.TryGetBox<GIBox.ClickSpawn>(out var box))
		{
			box.AttachTweener(UnityEngine.Object.Instantiate(lightningPrefab));
		}
	}

	private void AtBoxTimerComplete(IControlClocks sender)
	{
		GIBox.ClickSpawn clickSpawn = sender as GIBox.ClickSpawn;
		int num = modifyController.CalcMaxAmount(clickSpawn);
		clickSpawn.AddAmount(clickSpawn.Config.RestoreAmount, num);
		clickSpawn.Data.TimerActive = clickSpawn.Data.Amount < num;
		if (clickSpawn.Data.TimerActive)
		{
			clickSpawn.SetTweenTimer(GetBoxFullTimer(clickSpawn));
		}
		this.OnRefreshSpawner?.Invoke(clickSpawn);
	}

	private RefTimer GetBoxFullTimer(GIBox.ClickSpawn box)
	{
		return TimeMaster.GetRefTimer(modifyController.RestoreTime(box));
	}

	public ClickResult Interact(GameItem gameItem)
	{
		GIBox.ClickSpawn box = gameItem.GetBox<GIBox.ClickSpawn>();
		if (box == null || box.Data.Amount == 0 || Field.IsFull)
		{
			return ClickResult.None;
		}
		CurrencyType currencyType = eventEnergyModeTempService.TryGetInteractPriceType();
		if (!currencyProcessor.TrySpent(currencyType, box.Config.EnergyPrice))
		{
			switch (currencyType)
			{
			case CurrencyType.Energy:
				restoreEnergyPopupOpener.Open();
				break;
			case CurrencyType.EventEnergy:
				restoreEventEnergyPopupOpener.Open();
				break;
			}
			return ClickResult.None;
		}
		Point nearEmptyPoint = Field.GetNearEmptyPoint(gameItem.Coordinates);
		GIData gIData = null;
		if (box.Data.DropQueue != null)
		{
			gIData = box.Data.DropQueue[0];
			box.Data.DropQueue.RemoveAt(0);
			if (box.Data.DropQueue.Count == 0)
			{
				box.Data.DropQueue = null;
			}
		}
		else
		{
			gIData = Weights.GetWeightObject((IList<WeightNode<GIData>>)modifyController.RefreshModifySpawnPool(box));
		}
		GIData giData = gIData.Copy().SetCoordinates(nearEmptyPoint);
		GameItem gameItem2 = Field.CreateItem(giData);
		gameItem2.DoCreateFrom(gameItem.Position);
		Merge.Sounds.Play("spawn");
		int num = modifyController.CalcMaxAmount(box);
		bool flag = box.Data.Amount == num && box.Config.CanRestore;
		box.AddAmount(-1, num);
		this.OnClickSpawn?.Invoke(gameItem, gameItem2);
		if (box.Data.Amount == 0 && box.Config.DestroyType != 0)
		{
			Field.RemoveItem(gameItem);
			if (box.Config.DestroyType == GIDestroyType.Destroy)
			{
				GIGhost ghost = GIGhost.CreateGhost(gameItem);
				Tween tween = destructionTweenBuilder.BuildTween(ghost);
				tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
				{
					ghost.Destroy();
				});
			}
			if (box.Config.DestroyType == GIDestroyType.Transform)
			{
				Field.CreateItem(box.Config.DestroyResult.Copy().SetCoordinates(gameItem.Coordinates)).DoCreate();
				return ClickResult.None;
			}
			return ClickResult.Deselect;
		}
		if (flag)
		{
			box.SetTweenTimer(GetBoxFullTimer(box));
		}
		return ClickResult.None;
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		GIBox.ClickSpawn clickSpawnBox = box as GIBox.ClickSpawn;
		if (currencyProcessor.TrySpent(CurrencyType.Hard, clickSpawnBox.SpeedUpPrice))
		{
			BuyCallback();
			signalBus.TrySendSpendHardForRechargeSignal(new Cost(clickSpawnBox.SpeedUpPrice, CurrencyType.Hard));
			currencyAmplitudeAnalytic.SendSpentEvent(CurrencyType.Hard, clickSpawnBox.SpeedUpPrice, CurrencyAmplitudeAnalytic.SourceType.MergeRechargeClickSpawner, Field.CurrentField.Type, new CompositeIdentificator(box.Parent.Config.UniqId));
		}
		else
		{
			OpenBank();
		}
		void BuyCallback()
		{
			RefreshSpawner(clickSpawnBox);
		}
	}

	public void RefreshSpawner(GIBox.ClickSpawn clickSpawnBox)
	{
		clickSpawnBox.StopTimer();
	}

	public void OpenBank()
	{
		if (bankWindow == null)
		{
			bankWindow = windowsManager.Get<BankWindow>();
		}
		bankWindow.Open();
		signalBus.Fire<OpenTabSignal>(new OpenTabSignal(tabIDNoHards));
	}

	void ITimeBoostListener.AtTimeBoost(float value)
	{
		List<GameItem> list = Field.CurrentField.Field.Where((GameItem x) => x != null && x.Data.HasModule(GIModuleType.ClickSpawn)).ToList();
		List<GameItem> list2 = list.Where((GameItem x) => x.GetBox<GIBox.ClickSpawn>().Data.TimerActive).ToList();
		Debug.Log($"Spawners found: {list.Count}, With active timer: {list2.Count()}");
		_speedUpClickSpawnStrategy.PlaySpeedUp(list2, value);
	}

	void ITimeBoostListener.AtPlayAnim()
	{
		_speedUpClickSpawnStrategy.PlaySpeedUp();
	}

	private void AtItemUnlock(GameItem gi)
	{
		if (gi.TryGetBox<GIBox.ClickSpawn>(out var box))
		{
			box.ValidateEffect();
		}
	}
}
