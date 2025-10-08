using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.MergeCore.GameItemBox;
using GreenT.UI;
using Merge;
using Merge.Core.Masters;
using Merge.MotionDesign;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class LockedController : Controller<LockedController>, IActionModuleController, IModuleController, ICreateItemListener
{
	[SerializeField]
	private DestructionTweenBuilder destructionTweenBuilder;

	[SerializeField]
	private CurrencyType priceType = CurrencyType.Hard;

	private ICurrencyProcessor currencyProcessor;

	private IWindowsManager windowsManager;

	private SignalBus signalBus;

	private int tabIDNoHards;

	private BankWindow bankWindow;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.Locked;

	public DestructionTweenBuilder DestructionTweenBuilder => destructionTweenBuilder;

	int ICreateItemListener.Priority => Priority.VeryHigh;

	public event Action<GameItem> OnItemActionUnlock;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, ICurrencyProcessor currencyProcessor, SignalBus signalBus, IConstants<int> intConstants, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.windowsManager = windowsManager;
		this.currencyProcessor = currencyProcessor;
		tabIDNoHards = intConstants["banktab_no_hards"];
		this.signalBus = signalBus;
	}

	public override void Init()
	{
		Controller<MergeController>.Instance.OnMerge -= AtMerge;
		Controller<MergeController>.Instance.OnMerge += AtMerge;
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (item.Data.TryGetModule<ModuleDatas.Locked>(out var result))
		{
			Locked box = new Locked(result, null);
			item.AddBox(box);
		}
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		Locked locked = box as Locked;
		int unlockPrice = locked.Data.UnlockPrice;
		if (currencyProcessor.TrySpent(priceType, unlockPrice))
		{
			locked.RemoveMoveLocker();
			currencyAmplitudeAnalytic.SendSpentEvent(priceType, unlockPrice, CurrencyAmplitudeAnalytic.SourceType.MergeRechargeLocker, Field.CurrentField.Type);
			this.OnItemActionUnlock?.Invoke(locked.Parent);
		}
		else
		{
			OpenBank();
		}
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

	private void AtMerge(GameItem merged)
	{
		foreach (Point item in Field.GetNotEmptyTilesDonut(merged.Coordinates))
		{
			Locked box = Field.CurrentField.Field[item].GetBox<Locked>();
			if (box != null && box.Data.BlocksMerge)
			{
				box.Data.BlocksMerge = false;
				box.RemoveMergeLocker();
			}
		}
	}
}
