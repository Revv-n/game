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

public class BubbleController : Controller<BubbleController>, IActionModuleController, IModuleController, ICreateItemListener
{
	[SerializeField]
	private BubbleTweenCreator tweenCreator;

	[SerializeField]
	private CurrencyType priceType = CurrencyType.Hard;

	private BankWindow bankWindow;

	private IWindowsManager windowsManager;

	private ICurrencyProcessor currencyProcessor;

	private SignalBus signalBus;

	private int tabIDNoHards;

	private MergeFieldProvider mergeFieldProvider;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	public BubbleTweenCreator BubbleTweenBuilder => tweenCreator;

	GIModuleType IModuleController.ModuleType => GIModuleType.Bubble;

	int ICreateItemListener.Priority => Priority.VeryHigh;

	public event Action<GameItem> OnBubbleUnlock;

	public event Action<GameItem, GameItem> OnBubbleMiss;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, ICurrencyProcessor currencyProcessor, SignalBus signalBus, IConstants<int> intConstants, MergeFieldProvider mergeFieldProvider, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.windowsManager = windowsManager;
		this.currencyProcessor = currencyProcessor;
		tabIDNoHards = intConstants["banktab_no_hards"];
		this.signalBus = signalBus;
		this.mergeFieldProvider = mergeFieldProvider;
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (!item.Data.TryGetModule<ModuleDatas.Bubble>(out var result))
		{
			return;
		}
		if (result.MainTimer.IsDefault)
		{
			result.MainTimer = GetLifeTime(result);
			Merge.Sounds.Play("bubble_appearance");
		}
		if (result.MainTimer.IsCompleted)
		{
			if (mergeFieldProvider.TryGetFieldWithItem(item, out var data))
			{
				GIMaster.SwapItem(data, item, result.Rest.Copy().SetCoordinates(item.Coordinates));
			}
		}
		else
		{
			Bubble bubble = new Bubble(result, null);
			bubble.OnTimerComplete += AtBoxTimerComplete;
			bubble.StartTweenTimer(result.MainTimer);
			item.AddBox(bubble);
		}
	}

	private RefTimer GetLifeTime(ModuleDatas.Bubble mData)
	{
		return TimeMaster.GetRefTimer(mData.MainTimer.TotalTime);
	}

	private void AtBoxTimerComplete(IControlClocks sender)
	{
		Bubble bubble = sender as Bubble;
		MissBubble(bubble.Parent, bubble.Data);
		Controller<SelectionController>.Instance.ClearSelection();
	}

	private void MissBubble(GameItem item, ModuleDatas.Bubble data)
	{
		GIGhost gIGhost = GIGhost.CreateGhost(item);
		BubbleEffect effect = item.GetBox<Bubble>().Effect;
		effect.transform.SetParent(gIGhost.transform.parent);
		if (mergeFieldProvider.TryGetFieldWithItem(item, out var data2))
		{
			GameItem gameItem = GIMaster.SwapItem(data2, item, data.Rest.Copy().SetCoordinates(item.Coordinates));
			tweenCreator.PopFromBubble(gameItem, gIGhost, effect);
			if (gIGhost.gameObject.activeInHierarchy)
			{
				Merge.Sounds.Play("bubble_open");
			}
			this.OnBubbleMiss?.Invoke(item, gameItem);
		}
	}

	private void UnlockBubble(GameItem item)
	{
		Bubble box = item.GetBox<Bubble>();
		BubbleEffect effect = box.Effect;
		tweenCreator.UnlockBubble(item, effect);
		box.CancelBlocking();
		box.Kill();
		item.RemoveModule(GIModuleType.Bubble);
		this.OnBubbleUnlock?.Invoke(item);
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		Bubble bubbleBox = box as Bubble;
		if (currencyProcessor.TrySpent(priceType, bubbleBox.Data.OpenPrice))
		{
			if (priceType == CurrencyType.Hard && bubbleBox.Data.OpenPrice > 0)
			{
				signalBus.Fire(new SpendHardBubbleSignal(bubbleBox.Data.OpenPrice));
			}
			AtBuy();
			currencyAmplitudeAnalytic.SendSpentEvent(priceType, bubbleBox.Data.OpenPrice, CurrencyAmplitudeAnalytic.SourceType.MergeRechargeBubble, Controller<GameItemController>.Instance.CurrentField.Type);
		}
		else
		{
			OpenBank();
		}
		void AtBuy()
		{
			UnlockBubble(bubbleBox.Parent);
		}
	}

	public void OpenBank()
	{
		if (bankWindow == null)
		{
			bankWindow = windowsManager.Get<BankWindow>();
		}
		bankWindow.Open();
		signalBus.Fire(new OpenTabSignal(tabIDNoHards));
	}
}
