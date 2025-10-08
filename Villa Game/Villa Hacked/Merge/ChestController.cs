using System;
using DG.Tweening;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.MergeCore;
using GreenT.UI;
using Merge.Core.Masters;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace Merge;

public class ChestController : Controller<ChestController>, IActionModuleController, IModuleController, ICreateItemListener, ITimeBoostListener
{
	private GIBox.Chest openingBox;

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
	private CurrencyType priceType;

	private Vector3 startPosition;

	private ICurrencyProcessor currencyProcessor;

	private IWindowsManager windowsManager;

	private SignalBus signalBus;

	private int tabIDNoHards;

	private BankWindow bankWindow;

	private CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic;

	public bool IsTestChest;

	GIModuleType IModuleController.ModuleType => GIModuleType.Chest;

	public bool HasOpeningChest => openingBox != null;

	int ICreateItemListener.Priority => Priority.High;

	public event Action<GIBox.Chest> OnChestOpened;

	public event Action<GIBox.Chest> OnBecomeOpening;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, ICurrencyProcessor currencyProcessor, SignalBus signalBus, IConstants<int> intConstants, CurrencyAmplitudeAnalytic currencyAmplitudeAnalytic)
	{
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.windowsManager = windowsManager;
		this.currencyProcessor = currencyProcessor;
		tabIDNoHards = intConstants["banktab_no_hards"];
		this.signalBus = signalBus;
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (item.Config.TryGetModule<ModuleConfigs.Chest>(out var result))
		{
			ModuleDatas.Chest chest = item.Data.GetModule<ModuleDatas.Chest>();
			if (chest == null)
			{
				chest = new ModuleDatas.Chest();
				item.Data.Modules.Add(chest);
			}
			GIBox.Chest chest2 = new GIBox.Chest(chest, result);
			if (chest2.Data.IsOpeningNow)
			{
				openingBox = chest2;
			}
			if (!chest2.Data.AlreadyOpened)
			{
				SubscribeOpening(chest2);
			}
			item.SetIconClock();
			item.AddBox(chest2);
		}
	}

	private void SubscribeOpening(GIBox.Chest sender)
	{
		sender.OnFastComplete += AtBoxOpened;
		sender.OnTimerComplete += AtBoxOpened;
	}

	private void UnsubscribeOpening(GIBox.Chest sender)
	{
		if (sender != null)
		{
			sender.OnFastComplete -= AtBoxOpened;
			sender.OnTimerComplete -= AtBoxOpened;
		}
	}

	private void AtBoxOpened(IControlClocks sender)
	{
		UnsubscribeOpening(sender as GIBox.Chest);
		openingBox = null;
		this.OnChestOpened?.Invoke(sender as GIBox.Chest);
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		GIBox.Chest chestBox = box as GIBox.Chest;
		if (chestBox.Data.IsOpeningNow)
		{
			if (currencyProcessor.TrySpent(priceType, chestBox.SpeedUpPrice))
			{
				AtBuy();
				currencyAmplitudeAnalytic.SendSpentEvent(priceType, chestBox.SpeedUpPrice, CurrencyAmplitudeAnalytic.SourceType.MergeRechargeChest, Controller<GameItemController>.Instance.CurrentField.Type);
			}
			else
			{
				OpenBank();
			}
		}
		else
		{
			openingBox = chestBox;
			chestBox.SetTestState(IsTestChest);
			chestBox.BeginDefaultOpening();
			Sounds.Play("boost_action");
			this.OnBecomeOpening?.Invoke(chestBox);
		}
		void AtBuy()
		{
			chestBox.SpeedUp();
			openingBox = null;
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

	public void SetTestState(bool isTest)
	{
		IsTestChest = isTest;
	}

	void ITimeBoostListener.AtTimeBoost(float value)
	{
		if (openingBox != null)
		{
			RefSkipableTimer refSkipableTimer = new RefSkipableTimer(openingBox.TweenTimer.Timer.TotalTime, openingBox.TweenTimer.Timer.StartTime.AddSeconds(0f - value));
			if (refSkipableTimer.IsCompleted)
			{
				openingBox.FinishOpening();
				AtBoxOpened(openingBox);
			}
			else
			{
				openingBox.Kill();
				openingBox.BeginOpening(refSkipableTimer);
			}
		}
	}

	void ITimeBoostListener.AtPlayAnim()
	{
		if (openingBox != null)
		{
			Sequence sequence = DOTween.Sequence();
			Sequence sequence2 = DOTween.Sequence();
			startPosition = openingBox.Parent.transform.localPosition;
			for (int i = 0; i < amountDrig; i++)
			{
				Transform transform = openingBox.Parent.transform;
				float y = UnityEngine.Random.Range(transform.localPosition.y, transform.localPosition.y + radius * deltaPositionY);
				float x = UnityEngine.Random.Range(transform.localPosition.x - radius, transform.localPosition.x + radius);
				sequence2.Append(openingBox.Parent.transform.DOLocalMove(new Vector3(x, y, transform.localPosition.z), timeDrig));
			}
			sequence.Join(openingBox.Parent.transform.DOScale(endSize, timeDrig * (float)amountDrig));
			sequence.Join(sequence2);
			sequence.OnComplete(delegate
			{
				openingBox.Parent.transform.localScale = Vector3.one;
				openingBox.Parent.transform.localPosition = startPosition;
			});
			openingBox.Parent.AppendOuterTween(sequence);
			sequence.OnComplete(delegate
			{
				openingBox.Parent.transform.DOScale(1f, 0.3f).SetEase(Ease.InBack);
				openingBox.Parent.transform.DOLocalMove(startPosition, 0.3f).SetEase(Ease.InBack);
			});
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && openingBox != null && openingBox.Data.MainTimer.IsCompleted)
		{
			openingBox.FinishOpening();
		}
	}
}
