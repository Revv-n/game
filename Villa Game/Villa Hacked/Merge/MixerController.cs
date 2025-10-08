using System;
using System.Collections.Generic;
using DG.Tweening;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeStore;
using GreenT.UI;
using Merge.Core.Masters;
using Merge.MotionDesign;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace Merge;

public class MixerController : Controller<MixerController>, IClickInteractionController, IModuleController, IActionModuleController, ICreateItemListener, ITimeBoostListener
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
	private CurrencyType priceType;

	private GIBox.Mixer mixingBox;

	private ICurrencyProcessor currencyProcessor;

	private ModifyController modifyController;

	private IWindowsManager windowsManager;

	private SignalBus signalBus;

	private BankWindow bankWindow;

	private MixerReloader mixerReloader;

	private RecipeManager _recipeManager;

	private RestoreEnergyPopupOpener restoreEnergyPopupOpener;

	private RestoreEventEnergyPopupOpener restoreEventEnergyPopupOpener;

	private EventEnergyModeTempService eventEnergyModeTempService;

	private int tabIDNoHards;

	private Vector3 startPosition;

	int ICreateItemListener.Priority => Priority.High;

	GIModuleType IModuleController.ModuleType => GIModuleType.Mixer;

	private GameItemController Field => Controller<GameItemController>.Instance;

	public event Action<GameItem> OnUpdateMixer;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, ICurrencyProcessor currencyProcessor, SignalBus signalBus, IConstants<int> intConstants, ModifyController modifyController, MixerReloader mixerReloader, RestoreEnergyPopupOpener restoreEnergyPopupOpener, EventEnergyModeTempService eventEnergyModeTempService, RestoreEventEnergyPopupOpener restoreEventEnergyPopupOpener, RecipeManager recipeManager)
	{
		this.windowsManager = windowsManager;
		this.currencyProcessor = currencyProcessor;
		tabIDNoHards = intConstants["banktab_no_hards"];
		this.signalBus = signalBus;
		this.modifyController = modifyController;
		this.mixerReloader = mixerReloader;
		this.restoreEnergyPopupOpener = restoreEnergyPopupOpener;
		this.restoreEventEnergyPopupOpener = restoreEventEnergyPopupOpener;
		this.eventEnergyModeTempService = eventEnergyModeTempService;
		_recipeManager = recipeManager;
	}

	public override void Init()
	{
		mixerReloader.Initialize();
	}

	void ICreateItemListener.AtItemCreated(GameItem item, MergeField mergeField)
	{
		if (!item.Config.TryGetModule<ModuleConfigs.Mixer>(out var result))
		{
			return;
		}
		ModuleDatas.Mixer mixer = item.Data.GetModule<ModuleDatas.Mixer>();
		if (mixer == null)
		{
			mixer = new ModuleDatas.Mixer();
			item.Data.Modules.Add(mixer);
		}
		Func<int, int> calcMaxAmount = (int maxAmount) => modifyController.CalcMaxAmount(maxAmount, item);
		Func<float, float> calcMaxMixingTime = (float maxMixingTime) => modifyController.RestoreTime(maxMixingTime, item);
		GIBox.Mixer box = new GIBox.Mixer(mixer, result, calcMaxAmount, calcMaxMixingTime, _recipeManager);
		box.OnTimerActiveChange += delegate(bool state)
		{
			if (state)
			{
				box.OnFastComplete += AtBoxMixedBackground;
				box.OnTimerComplete += AtBoxMixedBackground;
			}
		};
		SubscribeMixerUpdating(box);
		item.SetIconClock();
		item.AddBox(box);
	}

	private void SubscribeMixerUpdating(GIBox.Mixer mixer)
	{
		mixer.OnUpdate += UpdateMixer;
	}

	private void UpdateMixer(GameItem gameItem)
	{
		this.OnUpdateMixer?.Invoke(gameItem);
	}

	public override void Preload()
	{
		base.Preload();
		Field.AfterItemCreated += AfterItemCreated;
	}

	private void AfterItemCreated(GameItem item)
	{
		if (item.TryGetBox<GIBox.Mixer>(out var box))
		{
			box.AttachTweener(UnityEngine.Object.Instantiate(lightningPrefab));
		}
	}

	private void SubscribeMixing(GIBox.Mixer sender)
	{
		sender.OnFastComplete += AtBoxMixed;
		sender.OnTimerComplete += AtBoxMixed;
	}

	private void UnsubscribeMixing(GIBox.Mixer sender)
	{
		if (sender != null)
		{
			sender.OnFastComplete -= AtBoxMixed;
			sender.OnTimerComplete -= AtBoxMixed;
		}
	}

	private void AtBoxMixed(IControlClocks sender)
	{
		GIBox.Mixer mixer = sender as GIBox.Mixer;
		UnsubscribeMixing(mixer);
		int modifiedMaxItemAmount = mixer.ModifiedMaxItemAmount;
		mixer.Data.Amount = modifiedMaxItemAmount;
		mixingBox = null;
	}

	private void AtBoxMixedBackground(IControlClocks sender)
	{
		GIBox.Mixer obj = sender as GIBox.Mixer;
		obj.OnFastComplete -= AtBoxMixedBackground;
		obj.OnTimerComplete -= AtBoxMixedBackground;
		int modifiedMaxItemAmount = obj.ModifiedMaxItemAmount;
		obj.Data.Amount = modifiedMaxItemAmount;
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		GIBox.Mixer mixerBox = box as GIBox.Mixer;
		if (mixerBox.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing)
		{
			if (mixerBox.SpeedUpPrice < 0)
			{
				return;
			}
			if (currencyProcessor.TrySpent(priceType, mixerBox.SpeedUpPrice))
			{
				if (priceType == CurrencyType.Hard)
				{
					signalBus.TrySendSpendHardForRechargeSignal(new Cost(mixerBox.SpeedUpPrice, CurrencyType.Hard));
				}
				AtBuy();
			}
			else
			{
				OpenBank();
			}
		}
		else
		{
			mixingBox = mixerBox;
			mixerBox.BeginDefaultMixing();
			Sounds.Play("boost_action");
			this.OnUpdateMixer?.Invoke(box.Parent);
		}
		void AtBuy()
		{
			SubscribeMixing(mixerBox);
			mixerBox.SpeedUp();
			mixingBox = null;
		}
	}

	public void RefreshMixer(GIBox.Mixer clickSpawnBox)
	{
		if (clickSpawnBox.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing)
		{
			clickSpawnBox.FinishMixing();
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

	public ClickResult Interact(GameItem gameItem)
	{
		GIBox.Mixer box = gameItem.GetBox<GIBox.Mixer>();
		if (box == null || Field.IsFull || box.Data.CurrentState != ModuleDatas.Mixer.StateMixer.Spawn)
		{
			return ClickResult.None;
		}
		CurrencyType currencyType = eventEnergyModeTempService.TryGetInteractPriceType();
		if (!currencyProcessor.TrySpent(currencyType, box.Config.Energy))
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
		GIData gIData = Weights.GetWeightObject((IList<WeightNode<GIData>>)modifyController.RefreshModifySpawnPool(box)).Copy();
		int modifiedMaxItemAmount = box.ModifiedMaxItemAmount;
		if (box.Data.Amount > modifiedMaxItemAmount)
		{
			box.Data.Amount = modifiedMaxItemAmount;
		}
		if (box.Data.Amount <= 0)
		{
			OnFinished(box, gameItem);
			if (box.Data.Amount == 0 && box.Config.DestroyType != 0)
			{
				return OnDestroyed(box, gameItem);
			}
		}
		else
		{
			if (box.Data.Amount > 0)
			{
				box.Data.ChangeValue(box.Data.Amount - 1);
				GIData giData = gIData.SetCoordinates(nearEmptyPoint);
				Field.CreateItem(giData).DoCreateFrom(gameItem.Position);
				Sounds.Play("spawn");
			}
			if (box.Data.Amount <= 0)
			{
				OnFinished(box, gameItem);
				if (box.Data.Amount == 0 && box.Config.DestroyType != 0)
				{
					return OnDestroyed(box, gameItem);
				}
			}
		}
		box.ValidateBlockLists();
		box.ValidateEffect();
		this.OnUpdateMixer?.Invoke(gameItem);
		return ClickResult.None;
	}

	private void OnFinished(GIBox.Mixer box, GameItem gameItem)
	{
		box.Data.CurrentState = ModuleDatas.Mixer.StateMixer.Wait;
		gameItem.GetBox<GIBox.Stack>().Clear();
		box.ClearRecipe();
	}

	private ClickResult OnDestroyed(GIBox.Mixer box, GameItem gameItem)
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

	void ITimeBoostListener.AtTimeBoost(float value)
	{
		if (mixingBox != null)
		{
			RefSkipableTimer refSkipableTimer = new RefSkipableTimer(mixingBox.Timer.Timer.TotalTime, mixingBox.Timer.Timer.StartTime.AddSeconds(0f - value));
			if (refSkipableTimer.IsCompleted)
			{
				mixingBox.FinishMixing();
				AtBoxMixed(mixingBox);
			}
			else
			{
				mixingBox.Kill();
				mixingBox.BeginMixing(refSkipableTimer);
			}
		}
	}

	void ITimeBoostListener.AtPlayAnim()
	{
		if (mixingBox != null)
		{
			Sequence sequence = DOTween.Sequence();
			Sequence sequence2 = DOTween.Sequence();
			startPosition = mixingBox.Parent.transform.localPosition;
			for (int i = 0; i < amountDrig; i++)
			{
				Transform transform = mixingBox.Parent.transform;
				float y = UnityEngine.Random.Range(transform.localPosition.y, transform.localPosition.y + radius * deltaPositionY);
				float x = UnityEngine.Random.Range(transform.localPosition.x - radius, transform.localPosition.x + radius);
				sequence2.Append(mixingBox.Parent.transform.DOLocalMove(new Vector3(x, y, transform.localPosition.z), timeDrig));
			}
			sequence.Join(mixingBox.Parent.transform.DOScale(endSize, timeDrig * (float)amountDrig));
			sequence.Join(sequence2);
			sequence.OnComplete(delegate
			{
				mixingBox.Parent.transform.localScale = Vector3.one;
				mixingBox.Parent.transform.localPosition = startPosition;
			});
			mixingBox.Parent.AppendOuterTween(sequence);
			sequence.OnComplete(delegate
			{
				mixingBox.Parent.transform.DOScale(1f, 0.3f).SetEase(Ease.InBack);
				mixingBox.Parent.transform.DOLocalMove(startPosition, 0.3f).SetEase(Ease.InBack);
			});
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && mixingBox != null && mixingBox.Data.MainTimer.IsCompleted)
		{
			mixingBox.FinishMixing();
		}
	}
}
