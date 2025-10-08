using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.MergeCore.GameItemBox;
using Merge.MotionDesign;
using UnityEngine;

namespace Merge;

public static class GIBox
{
	public abstract class Base
	{
		public GameItem Parent { get; private set; }

		public abstract GIModuleType ModuleType { get; }

		public void Attach(GameItem parent)
		{
			Parent = parent;
			AtAttach();
		}

		public void Save()
		{
			AtSave();
		}

		public abstract void Kill();

		protected virtual void AtAttach()
		{
		}

		protected virtual void AtSave()
		{
		}
	}

	public abstract class Casted<D, C> : Base where D : ModuleDatas.Base where C : ModuleConfigs.Base
	{
		public override GIModuleType ModuleType
		{
			get
			{
				if (Data != null)
				{
					return Data.ModuleType;
				}
				return Config.ModuleType;
			}
		}

		public D Data { get; private set; }

		public C Config { get; private set; }

		protected Casted(D data, C config)
		{
			Data = data;
			Config = config;
		}
	}

	public class AutoSpawn : Casted<ModuleDatas.AutoSpawn, ModuleConfigs.AutoSpawn>, IControlClocks, IActionModule, ISpeedUpReciver
	{
		private SpeedUpUnit speedUpUnit;

		public List<WeightNode<GIData>> ModifiedSpawnPool = new List<WeightNode<GIData>>();

		public int Amount
		{
			get
			{
				return base.Data.Amount;
			}
			set
			{
				SetAmount(value);
			}
		}

		public int SpeedUpPrice => Mathf.CeilToInt(base.Data.MainTimer.TimeLeft * base.Config.SecPrice);

		public int ClockControlPriority => 10;

		public TweenTimer TweenTimer { get; private set; }

		public LightningTweenBuilder LightningEffect { get; private set; }

		public bool IsTimerVisible
		{
			get
			{
				if (base.Data.TimerActive)
				{
					return base.Data.Amount == 0;
				}
				return false;
			}
		}

		public bool IsActionEnable => IsTimerVisible;

		public int ActionPriority => 4;

		public bool CanSpawn
		{
			get
			{
				if (base.Parent.AllowInteraction(GIModuleType.ClickSpawn))
				{
					return base.Data.Amount > 0;
				}
				return false;
			}
		}

		float ISpeedUpReciver.SpeedUpMultiplyer => speedUpUnit.CurrentSpeedUpValue;

		public event Action<bool> OnSpeedUpChange;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public bool IsMaxAmount(int maxAmount)
		{
			return base.Data.Amount == maxAmount;
		}

		public AutoSpawn(ModuleDatas.AutoSpawn mData, ModuleConfigs.AutoSpawn mConfig, int modifiedMaxAmount)
			: base(mData, mConfig)
		{
			RefSkipableTimer mainTimer = mData.GetMainTimer();
			speedUpUnit = new SpeedUpUnit(mainTimer, null);
			FixDataIn_1_2_0(this, modifiedMaxAmount);
			if (mData.TimerActive)
			{
				if (mData.SpeedupData != null)
				{
					float offlineTimeSkipped = speedUpUnit.GetOfflineTimeSkipped(mData.SpeedupData);
					Debug.Log(offlineTimeSkipped);
					mainTimer.Skipped += offlineTimeSkipped;
				}
				int num = mData.MainTimer.RemovePeriods();
				Amount += Mathf.Min(mData.Amount + mConfig.RestoreAmount * num, modifiedMaxAmount);
				base.Data.TimerActive = base.Data.Amount < modifiedMaxAmount;
				if (mData.TimerActive)
				{
					StartTweenTimer(mData.MainTimer);
				}
			}
			ModifiedSpawnPool = base.Config.SpawnPool.ToList();
		}

		public void AttachTweener(LightningTweenBuilder tb)
		{
			LightningEffect = tb;
			LightningEffect.transform.SetParent(base.Parent.transform);
			LightningEffect.transform.localPosition = Vector3.zero;
			LightningEffect.transform.SetScale(base.Parent.SizeMul);
			LightningEffect.BuildTween(base.Parent.IconRenderer.transform, withEnergy: false);
			ValidateEffect();
		}

		public void StartTweenTimer(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			base.Data.TimerActive = true;
			TweenTimer?.Kill();
			TweenTimer = new TweenTimer(base.Data.MainTimer, StopTimer, AtTimerTick);
			speedUpUnit.SetTimer(TweenTimer);
			if (base.Data.Amount == 0)
			{
				this.OnTimerActiveChange?.Invoke(obj: true);
			}
		}

		public void ValidateEffect()
		{
			LightningEffect.SetVisible(CanSpawn);
		}

		public void StartTimer(float restoreTime)
		{
			StartTweenTimer(TimeMaster.GetRefTimer(restoreTime));
		}

		public override void Kill()
		{
			LightningEffect.SetVisible(visible: false);
			TweenTimer?.Kill();
		}

		public void StopTimer()
		{
			TweenTimer?.Kill();
			TweenTimer = null;
			base.Data.TimerActive = false;
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
			ValidateEffect();
		}

		protected override void AtSave()
		{
			base.Data.SpeedupData = speedUpUnit.GetSave();
		}

		void ISpeedUpReciver.AddSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.AddSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.RemoveSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSourcesWithKey(string key)
		{
			speedUpUnit.RemoveSpeedUpSourcesWithKey(key);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		private void AtTimerTick(TimerStatus info)
		{
			this.OnTimerTick?.Invoke(info);
		}

		private void SetAmount(int amount)
		{
			bool isTimerVisible = IsTimerVisible;
			base.Data.Amount = amount;
			if (isTimerVisible != IsTimerVisible)
			{
				this.OnTimerActiveChange?.Invoke(IsTimerVisible);
			}
		}

		private void FixDataIn_1_2_0(AutoSpawn box, int modifiedMaxAmount)
		{
			if (box.Data.MainTimer.IsCompleted && !box.Data.TimerActive && box.Data.Amount == 0)
			{
				box.Data.Amount = modifiedMaxAmount;
				box.Data.MainTimer = TimeMaster.DefaultTimer;
			}
		}
	}

	public class AutoUpgrade : Casted<ModuleDatas.AutoUpgrade, ModuleConfigs.AutoUpgrade>, IControlClocks
	{
		public int ClockControlPriority => 30;

		public TweenTimer Timer { get; private set; }

		public bool IsTimerVisible => true;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public AutoUpgrade(ModuleDatas.AutoUpgrade data, ModuleConfigs.AutoUpgrade config)
			: base(data, config)
		{
		}

		public void StartTweenTimer(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			Timer = new TweenTimer(base.Data.MainTimer, StopTimer, AtTimerTick);
			this.OnTimerActiveChange?.Invoke(obj: true);
		}

		public void StartDefaultTimer()
		{
			StartTweenTimer(TimeMaster.GetRefTimer(base.Config.Time));
		}

		public override void Kill()
		{
			Timer?.Kill();
		}

		private void AtTimerTick(TimerStatus info)
		{
			this.OnTimerTick?.Invoke(info);
		}

		private void StopTimer()
		{
			Timer?.Kill();
			Timer = null;
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
		}
	}

	public class Bubble : Casted<ModuleDatas.Bubble, ModuleConfigs.Bubble>, IControlClocks, IActionModule, IBlockModulesAction, IBlockModulesInteraction
	{
		private FilterNode<GIModuleType> actionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Bubble);

		private FilterNode<GIModuleType> interactionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Bubble);

		public BubbleEffect Effect { get; private set; }

		public int ClockControlPriority => -10;

		public TweenTimer Timer { get; private set; }

		public bool IsTimerVisible => false;

		bool IActionModule.IsActionEnable => true;

		int IActionModule.ActionPriority => 100;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public event Action OnBlockActionChange;

		public event Action OnBlockInteractionChange;

		public Bubble(ModuleDatas.Bubble data, ModuleConfigs.Bubble config)
			: base(data, config)
		{
		}

		public void CancelBlocking()
		{
			actionsFilter = FilterNode<GIModuleType>.Empty;
			interactionsFilter = FilterNode<GIModuleType>.Empty;
			this.OnBlockActionChange?.Invoke();
			this.OnBlockInteractionChange?.Invoke();
		}

		public void StartTweenTimer(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			Timer = new TweenTimer(base.Data.MainTimer, CompleteTimer, AtTimerTick);
			this.OnTimerActiveChange?.Invoke(obj: true);
		}

		public void StartDefaultTimer()
		{
			StartTweenTimer(TimeMaster.GetRefTimer(base.Data.MainTimer.TotalTime));
		}

		public override void Kill()
		{
			Timer?.Kill();
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			PushInBubble();
		}

		private void PushInBubble()
		{
			Effect = Controller<BubbleController>.Instance.BubbleTweenBuilder.CreateBubbleEffect(base.Parent);
		}

		private void AtTimerTick(TimerStatus info)
		{
			this.OnTimerTick?.Invoke(info);
			if (base.Data.MainTimer.IsCompleted)
			{
				CompleteTimer();
			}
		}

		private void CompleteTimer()
		{
			Timer?.Kill();
			Timer = null;
			Effect.BubbleTween.Kill();
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
		}
	}

	public class Chest : Casted<ModuleDatas.Chest, ModuleConfigs.Chest>, IBlockModulesInteraction, IActionModule, IControlClocks, IBlockModulesAction, ISpeedUpReciver
	{
		private FilterNode<GIModuleType> interactionsFilter;

		private FilterNode<GIModuleType> actionsFilter;

		private SpeedUpUnit speedUpUnit;

		public bool IsTestChest;

		int IControlClocks.ClockControlPriority => 100;

		bool IControlClocks.IsTimerVisible
		{
			get
			{
				if (base.Config.IsOpenable)
				{
					return base.Data.IsOpeningNow;
				}
				return false;
			}
		}

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

		bool IActionModule.IsActionEnable
		{
			get
			{
				if (base.Config.IsOpenable)
				{
					return !base.Data.AlreadyOpened;
				}
				return false;
			}
		}

		int IActionModule.ActionPriority => 100;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		public int SpeedUpPrice => Mathf.Max(0, Mathf.RoundToInt(base.Config.PriceMul * base.Data.MainTimer.TimeLeft));

		public TweenTimer TweenTimer { get; private set; }

		float ISpeedUpReciver.SpeedUpMultiplyer => speedUpUnit.CurrentSpeedUpValue;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public event Action OnBlockInteractionChange;

		public event Action OnBlockActionChange;

		public event Action<Chest> OnFastComplete;

		public event Action<bool> OnSpeedUpChange;

		public Chest(ModuleDatas.Chest data, ModuleConfigs.Chest config)
			: base(data, config)
		{
			RefSkipableTimer mainTimer = data.GetMainTimer();
			speedUpUnit = new SpeedUpUnit(mainTimer, null);
			if (base.Data.IsOpeningNow)
			{
				if (data.SpeedupData != null)
				{
					float offlineTimeSkipped = speedUpUnit.GetOfflineTimeSkipped(data.SpeedupData);
					mainTimer.Skipped += offlineTimeSkipped;
				}
				if (base.Data.MainTimer.IsCompleted)
				{
					base.Data.IsOpeningNow = false;
					base.Data.AlreadyOpened = true;
				}
				else
				{
					BeginOpening(base.Data.MainTimer);
				}
			}
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			ValidateBlockLists();
		}

		private void ValidateBlockLists()
		{
			actionsFilter = (base.Data.AlreadyOpened ? FilterNode<GIModuleType>.Empty : new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Chest, GIModuleType.Sell));
			if (base.Data.AlreadyOpened)
			{
				interactionsFilter = FilterNode<GIModuleType>.Empty;
			}
			else if (base.Data.IsOpeningNow)
			{
				interactionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Chest);
			}
			else
			{
				interactionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Chest, GIModuleType.Merge);
			}
		}

		public void SetTestState(bool isTest)
		{
			IsTestChest = isTest;
		}

		public void BeginDefaultOpening()
		{
			BeginOpening(IsTestChest ? TimeMaster.GetRefTimer(60f) : TimeMaster.GetRefTimer(base.Config.TimeToOpen));
		}

		public void BeginOpening(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			base.Data.IsOpeningNow = true;
			TweenTimer = new TweenTimer(timer, FinishOpening, AtTimerTick);
			speedUpUnit.SetTimer(TweenTimer);
			ValidateBlockLists();
			this.OnTimerActiveChange?.Invoke(obj: true);
		}

		private void AtTimerTick(TimerStatus info)
		{
			this.OnTimerTick?.Invoke(info);
			if (base.Data.MainTimer.IsCompleted)
			{
				FinishOpening();
			}
		}

		public void SpeedUp()
		{
			FinishOpening();
			this.OnFastComplete?.Invoke(this);
		}

		public void FinishOpening()
		{
			TweenTimer?.Kill();
			TweenTimer = null;
			base.Data.IsOpeningNow = false;
			base.Data.AlreadyOpened = true;
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
			ValidateBlockLists();
			this.OnBlockInteractionChange?.Invoke();
			this.OnBlockActionChange?.Invoke();
		}

		public override void Kill()
		{
			TweenTimer?.Kill();
		}

		protected override void AtSave()
		{
			base.Data.SpeedupData = speedUpUnit.GetSave();
		}

		void ISpeedUpReciver.AddSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.AddSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.RemoveSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSourcesWithKey(string key)
		{
			speedUpUnit.RemoveSpeedUpSourcesWithKey(key);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}
	}

	public class ClickSpawn : Casted<ModuleDatas.ClickSpawn, ModuleConfigs.ClickSpawn>, IControlClocks, IActionModule, ISpeedUpReciver
	{
		public List<WeightNode<GIData>> ModifiedSpawnPool = new List<WeightNode<GIData>>();

		private SpeedUpUnit speedUpUnit;

		public int ClockControlPriority => 10;

		public TweenTimer TweenTimer { get; private set; }

		public bool IsTimerVisible
		{
			get
			{
				if (base.Data.TimerActive)
				{
					return base.Data.Amount == 0;
				}
				return false;
			}
		}

		bool IActionModule.IsActionEnable => IsTimerVisible;

		int IActionModule.ActionPriority => 5;

		public int SpeedUpPrice => Mathf.CeilToInt(base.Data.MainTimer.TimeLeft * base.Config.SpeedUpMul);

		public LightningTweenBuilder LightningEffect { get; private set; }

		public bool CanSpawn
		{
			get
			{
				if (base.Parent.AllowInteraction(GIModuleType.ClickSpawn))
				{
					return base.Data.Amount > 0;
				}
				return false;
			}
		}

		float ISpeedUpReciver.SpeedUpMultiplyer => speedUpUnit.CurrentSpeedUpValue;

		public event Action<bool> OnSpeedUpChange;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public ClickSpawn(ModuleDatas.ClickSpawn mData, ModuleConfigs.ClickSpawn mConfig, int modifiedMaxAmount)
			: base(mData, mConfig)
		{
			if (!mData.TimerActive && mData.Amount < modifiedMaxAmount && mConfig.CanRestore)
			{
				mData.TimerActive = true;
			}
			RefSkipableTimer mainTimer = mData.GetMainTimer();
			if (mainTimer.IsDefault)
			{
				mData.MainTimer = new RefSkipableTimer(mConfig.RestoreTime);
			}
			speedUpUnit = new SpeedUpUnit(mainTimer, null);
			if (mData.TimerActive)
			{
				if (mData.SpeedupData != null)
				{
					float offlineTimeSkipped = speedUpUnit.GetOfflineTimeSkipped(mData.SpeedupData);
					mainTimer.Skipped += offlineTimeSkipped;
				}
				int num = mData.MainTimer.RemovePeriods();
				mData.Amount = Mathf.Min(mData.Amount + mConfig.RestoreAmount * num, modifiedMaxAmount);
				mData.TimerActive = mData.Amount < modifiedMaxAmount;
				mData.WasRefreshedOffline = num > 0 && !mData.TimerActive;
				if (mData.TimerActive)
				{
					SetTweenTimer(mData.MainTimer);
				}
			}
			else
			{
				mData.WasRefreshedOffline = false;
			}
			ModifiedSpawnPool = base.Config.SpawnPool.Select((WeightNode<GIData> x) => new WeightNode<GIData>(x.value.Copy(), x.Weight)).ToList();
		}

		public void SetTweenTimer(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			base.Data.TimerActive = true;
			TweenTimer?.Kill();
			TweenTimer = new TweenTimer(base.Data.MainTimer, StopTimer, AtTimerTick);
			speedUpUnit.SetTimer(TweenTimer);
			if (base.Data.Amount == 0)
			{
				this.OnTimerActiveChange?.Invoke(obj: true);
			}
		}

		public void AttachTweener(LightningTweenBuilder tb)
		{
			LightningEffect = tb;
			LightningEffect.transform.SetParent(base.Parent.transform);
			LightningEffect.transform.localPosition = Vector3.zero;
			LightningEffect.transform.SetScale(base.Parent.SizeMul);
			LightningEffect.BuildTween(base.Parent.IconRenderer.transform, base.Config.EnergyPrice > 0);
			ValidateEffect();
			if (!base.Parent.AllowInteraction(GIModuleType.ClickSpawn))
			{
				base.Parent.OnBlockInteractionChange += UnlockCallback;
			}
		}

		private void AtTimerTick(TimerStatus info)
		{
			this.OnTimerTick?.Invoke(info);
			if (base.Data.MainTimer.IsCompleted)
			{
				StopTimer();
			}
		}

		public void StopTimer()
		{
			if (IsTimerVisible)
			{
				base.Data.InvokeOnTimerComplete();
			}
			LightningEffect?.Kill();
			TweenTimer?.Kill();
			TweenTimer = null;
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
		}

		public void AddAmount(int toAdd, int modifiedMaxAmount = -1)
		{
			int num = ((modifiedMaxAmount > 0) ? modifiedMaxAmount : base.Config.MaxAmount);
			int max = ((base.Data.DropQueue == null) ? num : Mathf.Max(base.Data.DropQueue.Count, num));
			base.Data.Amount = Mathf.Clamp(base.Data.Amount + toAdd, 0, max);
			if (!(LightningEffect == null))
			{
				LightningEffect.SetVisible(CanSpawn);
				this.OnTimerActiveChange?.Invoke(base.Data.Amount == 0 && base.Config.CanRestore);
			}
		}

		public override void Kill()
		{
			LightningEffect?.Kill();
			TweenTimer?.Kill();
			TweenTimer = null;
			this.OnTimerActiveChange = null;
			this.OnTimerComplete = null;
		}

		public void ValidateEffect()
		{
			LightningEffect.SetVisible(CanSpawn);
		}

		private void UnlockCallback(GameItem sender)
		{
			if (base.Parent.AllowInteraction(GIModuleType.ClickSpawn))
			{
				base.Parent.OnBlockInteractionChange -= UnlockCallback;
				ValidateEffect();
			}
		}

		protected override void AtSave()
		{
			base.Data.SpeedupData = speedUpUnit.GetSave();
		}

		void ISpeedUpReciver.AddSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.AddSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.RemoveSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSourcesWithKey(string key)
		{
			speedUpUnit.RemoveSpeedUpSourcesWithKey(key);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}
	}

	public class Locked : Casted<ModuleDatas.Locked, ModuleConfigs.Locked>, IMotionBlocker, IBlockModulesInteraction, IBlockModulesAction, IActionModule
	{
		private GameObject moveLocker;

		private GameObject mergeLocker;

		private SpriteRenderer lockedIcon;

		private FilterNode<GIModuleType> interactionsFilter;

		private FilterNode<GIModuleType> actionsFilter;

		private Tween removeMoveLockTween;

		private Tween removeMergeLockTween;

		private Tile tile;

		private Vector2 imageFix = new Vector2(1.75f, 1.75f);

		private Vector2 imageBoxFix = new Vector2(1.64f, 1.64f);

		public SpriteRenderer LockedIcon => lockedIcon;

		public bool BlockMotion => true;

		private TileController TC => Controller<TileController>.Instance;

		bool IActionModule.IsActionEnable => true;

		int IActionModule.ActionPriority => 100;

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		private Color LockedColor => new Color(1f, 1f, 1f);

		public event Action OnBlockInteractionChange;

		public event Action OnBlockActionChange;

		public Locked(ModuleDatas.Locked data, ModuleConfigs.Locked config)
			: base(data, config)
		{
		}

		public override void Kill()
		{
			removeMoveLockTween?.Kill(complete: true);
			removeMergeLockTween?.Kill(complete: true);
			SetBlockTileBack(isLock: false);
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			interactionsFilter = (base.Data.BlocksMerge ? new FilterNode<GIModuleType>(FilterType.White) : new FilterNode<GIModuleType>(FilterType.White, default(GIModuleType)));
			actionsFilter = (base.Data.BlocksMerge ? new FilterNode<GIModuleType>(FilterType.White) : new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Locked));
			AddMoveLocker();
			if (base.Data.BlocksMerge)
			{
				AddMergeLocker();
			}
			SetBlockTileBack(isLock: true);
		}

		private void SetBlockTileBack(bool isLock)
		{
			if (TC.TryGetTile(base.Parent.Coordinates, out tile))
			{
				TC.SetTileBack(tile, isLock);
			}
		}

		private void AddMoveLocker()
		{
			moveLocker = new GameObject("Web");
			base.Parent.AddChild(moveLocker.transform, atCentre: true);
			SpriteRenderer spriteRenderer = moveLocker.AddComponent<SpriteRenderer>();
			spriteRenderer.drawMode = SpriteDrawMode.Sliced;
			spriteRenderer.size = imageFix;
			spriteRenderer.SetSprite(IconProvider.GetSprite("Images/Web")).SetOrder("Game", 1);
			moveLocker.layer = 9;
			spriteRenderer.SetAlpha(0.7f);
			moveLocker.transform.localPosition = Vector3.zero;
			base.Parent.IconRenderer.color = LockedColor;
			moveLocker.transform.Translate(0f, 0f, -0.1f);
		}

		private void AddMergeLocker()
		{
			mergeLocker = new GameObject("LockBox");
			base.Parent.AddChild(mergeLocker.transform, atCentre: true);
			lockedIcon = mergeLocker.AddComponent<SpriteRenderer>();
			if (base.Data.RandomSkin == 0)
			{
				base.Data.RandomSkin = UnityEngine.Random.Range(1, 5);
			}
			lockedIcon.SetSprite(IconProvider.GetSprite($"Images/LockBox{base.Data.RandomSkin}")).SetOrder("Game", 1);
			lockedIcon.drawMode = SpriteDrawMode.Sliced;
			lockedIcon.size = imageBoxFix;
			mergeLocker.layer = 9;
			mergeLocker.transform.localPosition = Vector3.zero;
			base.Parent.IconRenderer.color = LockedColor;
			mergeLocker.transform.Translate(0f, 0f, -0.11f);
			moveLocker?.SetActive(value: false);
			base.Parent.IconRenderer.enabled = false;
		}

		public void RemoveMoveLocker()
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Join(DOTweenModuleSprite.DOColor(base.Parent.IconRenderer, Color.white, 0.3f));
			sequence.Join(moveLocker.transform.DOScale(0f, 0.3f));
			sequence.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(moveLocker);
			});
			removeMoveLockTween = sequence;
			interactionsFilter = FilterNode<GIModuleType>.Empty;
			actionsFilter = FilterNode<GIModuleType>.Empty;
			this.OnBlockInteractionChange?.Invoke();
			this.OnBlockActionChange?.Invoke();
			base.Parent.RemoveModule(GIModuleType.Locked);
		}

		public void RemoveMergeLocker()
		{
			base.Parent.IconRenderer.enabled = true;
			moveLocker?.SetActive(value: true);
			GIGhost ghost = GIGhost.CreateGhost(mergeLocker.GetComponent<SpriteRenderer>());
			ghost.transform.SetParent(base.Parent.transform.parent);
			base.Parent.transform.SetScale(0f);
			Sequence sequence = DOTween.Sequence();
			Tween tween = Controller<LockedController>.Instance.DestructionTweenBuilder.BuildTween(ghost);
			sequence.Join(tween);
			sequence.Join(base.Parent.transform.DOScale(1f, 0.6f)).SetEase(Ease.InSine);
			tween.onComplete = (TweenCallback)Delegate.Combine(tween.onComplete, (TweenCallback)delegate
			{
				ghost?.Destroy();
				ghost = null;
			});
			sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
			{
				ghost?.Destroy();
				ghost = null;
			});
			base.Parent.AppendOuterTween(sequence);
			UnityEngine.Object.Destroy(mergeLocker);
			removeMergeLockTween = tween;
			base.Data.BlocksMerge = false;
			interactionsFilter = new FilterNode<GIModuleType>(FilterType.White, default(GIModuleType));
			actionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Locked);
			this.OnBlockInteractionChange?.Invoke();
			this.OnBlockActionChange?.Invoke();
		}
	}

	public class Mixer : Casted<ModuleDatas.Mixer, ModuleConfigs.Mixer>, IBlockModulesInteraction, IActionModule, IControlClocks, IBlockModulesAction, ISpeedUpReciver
	{
		private LightningTweenBuilder lightningPrefab;

		private Func<int, int> calcMaxAmount;

		private Func<float, float> calcMaxMixingTime;

		private RecipeModel activeRecipe;

		private readonly RecipeManager _recipeManager;

		private FilterNode<GIModuleType> interactionsFilter;

		private FilterNode<GIModuleType> actionsFilter;

		private SpeedUpUnit speedUpUnit;

		public LightningTweenBuilder LightningEffect { get; private set; }

		public bool CanSpawn
		{
			get
			{
				if (base.Parent.AllowInteraction(GIModuleType.Mixer))
				{
					return base.Data.Amount > 0;
				}
				return false;
			}
		}

		public RecipeModel ActiveRecipe
		{
			get
			{
				if (base.Data.ActiveRecipeID.HasValue && activeRecipe == null)
				{
					activeRecipe = GetActiveRecipe();
					if (activeRecipe == null)
					{
						base.Data.ActiveRecipeID = null;
					}
				}
				if (!base.Data.ActiveRecipeID.HasValue)
				{
					activeRecipe = null;
				}
				return activeRecipe;
			}
		}

		public float SecondPrice => ActiveRecipe?.SecondPrice ?? 0f;

		public int ModifiedMaxMixingTime
		{
			get
			{
				if (HasRecipe())
				{
					int time = GetActiveRecipe().Time;
					return (int)calcMaxMixingTime(time);
				}
				SendRecipeErrorMessage();
				return 0;
			}
		}

		public int ModifiedMaxItemAmount
		{
			get
			{
				if (HasRecipe())
				{
					int outCount = _recipeManager.Collection.FirstOrDefault((RecipeModel model) => model.ID == base.Data.ActiveRecipeID.Value).OutCount;
					return calcMaxAmount(outCount);
				}
				SendRecipeErrorMessage();
				return 1;
			}
		}

		public List<WeightNode<GIData>> ModifiedMixPool { get; set; }

		int IControlClocks.ClockControlPriority => 100;

		bool IControlClocks.IsTimerVisible => base.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing;

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

		bool IActionModule.IsActionEnable => base.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing;

		int IActionModule.ActionPriority => 100;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		public int SpeedUpPrice
		{
			get
			{
				int num = Mathf.CeilToInt(base.Data.MainTimer.TimeLeft * SecondPrice);
				if (num > 0)
				{
					return num;
				}
				return ModifiedMaxItemAmount;
			}
		}

		public TimerBase Timer { get; private set; }

		float ISpeedUpReciver.SpeedUpMultiplyer => speedUpUnit.CurrentSpeedUpValue;

		public event Action<GameItem> OnUpdate;

		public event Action<bool> OnTimerActiveChange;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<TimerStatus> OnTimerTick;

		public event Action OnBlockInteractionChange;

		public event Action OnBlockActionChange;

		public event Action<Mixer> OnFastComplete;

		public event Action<bool> OnSpeedUpChange;

		public Mixer(ModuleDatas.Mixer data, ModuleConfigs.Mixer config, Func<int, int> calcMaxAmount, Func<float, float> calcMaxMixingTime, RecipeManager recipeManager)
			: base(data, config)
		{
			_recipeManager = recipeManager;
			ModifiedMixPool = GetModifiedMixPool();
			this.calcMaxAmount = calcMaxAmount;
			this.calcMaxMixingTime = calcMaxMixingTime;
			RefSkipableTimer mainTimer = data.GetMainTimer();
			if (mainTimer.IsDefault)
			{
				data.MainTimer = new RefSkipableTimer(ModifiedMaxMixingTime);
			}
			speedUpUnit = new SpeedUpUnit(mainTimer, null);
			if (data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing)
			{
				if (data.SpeedupData != null)
				{
					float offlineTimeSkipped = speedUpUnit.GetOfflineTimeSkipped(data.SpeedupData);
					mainTimer.Skipped += offlineTimeSkipped;
				}
				int a = Mathf.Max(0, Mathf.RoundToInt(data.MainTimer.Passed / (float)(ActiveRecipe.Time / ActiveRecipe.OutCount)));
				data.Amount = Mathf.Min(a, ModifiedMaxItemAmount);
				if (data.MainTimer.IsCompleted)
				{
					FinishMixing();
					return;
				}
				SetTweenTimer(data.MainTimer);
				BeginMixing(data.MainTimer);
			}
		}

		public void ClearRecipe()
		{
			base.Data.ActiveRecipeID = null;
			activeRecipe = null;
		}

		public void SetTweenTimer(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			base.Data.CurrentState = ModuleDatas.Mixer.StateMixer.Mixing;
			Timer?.Kill();
			Timer = new TweenTimer(base.Data.MainTimer, FinishMixing, AtTimerTick);
			speedUpUnit.SetTimer(Timer);
			if (base.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing)
			{
				this.OnTimerActiveChange?.Invoke(obj: true);
			}
		}

		public void AttachTweener(LightningTweenBuilder tb)
		{
			LightningEffect = tb;
			LightningEffect.transform.SetParent(base.Parent.transform);
			LightningEffect.transform.localPosition = Vector3.zero;
			LightningEffect.transform.SetScale(base.Parent.SizeMul);
			LightningEffect.BuildTween(base.Parent.IconRenderer.transform, base.Config.Energy > 0);
			ValidateEffect();
			if (!base.Parent.AllowInteraction(GIModuleType.Mixer))
			{
				base.Parent.OnBlockInteractionChange += UnlockCallback;
			}
		}

		private void UnlockCallback(GameItem sender)
		{
			if (base.Parent.AllowInteraction(GIModuleType.Mixer))
			{
				base.Parent.OnBlockInteractionChange -= UnlockCallback;
				ValidateEffect();
			}
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			ValidateBlockLists();
			ValidateEffect();
		}

		public void ValidateBlockLists()
		{
			FilterNode<GIModuleType> filterNode = FilterNode<GIModuleType>.Empty;
			FilterNode<GIModuleType> filterNode2 = FilterNode<GIModuleType>.Empty;
			switch (base.Data.CurrentState)
			{
			case ModuleDatas.Mixer.StateMixer.None:
				filterNode = FilterNode<GIModuleType>.Empty;
				filterNode2 = FilterNode<GIModuleType>.Empty;
				break;
			case ModuleDatas.Mixer.StateMixer.Wait:
				filterNode = FilterNode<GIModuleType>.Empty;
				filterNode2 = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Stack, GIModuleType.Merge);
				break;
			case ModuleDatas.Mixer.StateMixer.Mixing:
				filterNode = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Mixer);
				filterNode2 = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Mixer, GIModuleType.Merge);
				break;
			case ModuleDatas.Mixer.StateMixer.Spawn:
				filterNode = FilterNode<GIModuleType>.Empty;
				filterNode2 = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Mixer, GIModuleType.Merge);
				break;
			}
			actionsFilter = filterNode;
			interactionsFilter = filterNode2;
			this.OnBlockActionChange?.Invoke();
			this.OnBlockInteractionChange?.Invoke();
		}

		public void BeginDefaultMixing()
		{
			BeginMixing(new RefSkipableTimer(ModifiedMaxMixingTime));
		}

		public void BeginMixing(RefTimer timer)
		{
			base.Data.MainTimer = timer;
			base.Data.CurrentState = ModuleDatas.Mixer.StateMixer.Mixing;
			Timer?.Kill();
			Timer = new TweenTimer(timer, FinishMixing, AtTimerTick);
			speedUpUnit.SetTimer(Timer);
			ModifiedMixPool = GetModifiedMixPool();
			ValidateBlockLists();
			this.OnTimerActiveChange?.Invoke(obj: true);
		}

		private void AtTimerTick(TimerStatus info)
		{
			int num = (int)(base.Data.MainTimer.Passed / (float)(ActiveRecipe.Time / ModifiedMaxItemAmount));
			int num2 = 9000000;
			_ = base.Data.MainTimer.Passed;
			_ = (float)num2;
			if (num > ModifiedMaxItemAmount)
			{
				num = ModifiedMaxItemAmount;
			}
			if (num > base.Data.Amount)
			{
				base.Data.Amount = num;
				this.OnUpdate?.Invoke(base.Parent);
			}
			this.OnTimerTick?.Invoke(info);
			if (base.Data.MainTimer.IsCompleted && !base.Data.MainTimer.IsDefault && base.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Mixing)
			{
				base.Data.MainTimer = TimeMaster.DefaultTimer;
				FinishMixing();
			}
		}

		public void StopTimer()
		{
			Timer?.Kill();
			Timer = null;
			this.OnTimerComplete?.Invoke(this);
			this.OnTimerActiveChange?.Invoke(obj: false);
		}

		public void SpeedUp()
		{
			FinishMixing();
			this.OnFastComplete?.Invoke(this);
		}

		public void FinishMixing()
		{
			Timer?.Kill();
			Timer = null;
			base.Data.CurrentState = ModuleDatas.Mixer.StateMixer.Spawn;
			base.Data.Amount = ModifiedMaxItemAmount;
			this.OnTimerActiveChange?.Invoke(obj: false);
			this.OnTimerComplete?.Invoke(this);
			ValidateBlockLists();
			ValidateEffect();
			this.OnBlockInteractionChange?.Invoke();
			this.OnBlockActionChange?.Invoke();
			this.OnUpdate?.Invoke(base.Parent);
		}

		public override void Kill()
		{
			Timer?.Kill();
			LightningEffect?.Kill();
		}

		public void ValidateEffect()
		{
			LightningEffect?.SetVisible(CanSpawn);
		}

		protected override void AtSave()
		{
			base.Data.SpeedupData = speedUpUnit.GetSave();
		}

		void ISpeedUpReciver.AddSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.AddSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSource(ISpeedUpSource source)
		{
			speedUpUnit.RemoveSpeedUpSource(source);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		void ISpeedUpReciver.RemoveSpeedUpSourcesWithKey(string key)
		{
			speedUpUnit.RemoveSpeedUpSourcesWithKey(key);
			this.OnSpeedUpChange(speedUpUnit.CurrentSpeedUpValue > 1f);
		}

		private bool HasRecipe()
		{
			if (base.Data.ActiveRecipeID.HasValue)
			{
				return _recipeManager.Collection.Any((RecipeModel item) => item.ID == base.Data.ActiveRecipeID.Value);
			}
			return false;
		}

		private RecipeModel GetActiveRecipe()
		{
			return _recipeManager.Collection.FirstOrDefault((RecipeModel item) => item.ID == base.Data.ActiveRecipeID.Value);
		}

		private void SendRecipeErrorMessage()
		{
			_ = base.Data.ActiveRecipeID.HasValue;
		}

		private List<WeightNode<GIData>> GetModifiedMixPool()
		{
			try
			{
				if (ActiveRecipe?.Result != null)
				{
					return ActiveRecipe?.Result.Select((WeightNode<GIData> x) => new WeightNode<GIData>(x.value.Copy(), x.Weight)).ToList();
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				throw;
			}
			SendRecipeErrorMessage();
			return new List<WeightNode<GIData>>();
		}
	}

	public class Sell : Casted<ModuleDatas.Sell, ModuleConfigs.Sell>, IActionModule
	{
		public int ActionPriority => 50;

		public bool IsActionEnable => true;

		public Sell(ModuleDatas.Sell data, ModuleConfigs.Sell config)
			: base(data, config)
		{
		}

		public override void Kill()
		{
		}
	}

	public class Tesla : Casted<ModuleDatas.Tesla, ModuleConfigs.Tesla>, ISpeedUpSource, IActionModule, IControlClocks, IBlockModulesInteractionReasonable, IBlockModulesInteraction, IBlockModulesAction
	{
		private SpeedUpSourceInfo speedUpSourceInfo;

		private GameObject effect;

		private TweenTimer lifeTimer;

		private FilterNode<GIModuleType> filterNode;

		private FilterNode<GIModuleType> actionsFilter;

		SpeedUpSourceInfo ISpeedUpSource.SpeedUpInfo => speedUpSourceInfo;

		bool IActionModule.IsActionEnable => true;

		int IActionModule.ActionPriority => 5;

		public int ClockControlPriority => 10;

		public bool IsTimerVisible => base.Data.Activated;

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => filterNode;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		public event Action<IControlClocks> OnTimerComplete;

		public event Action<bool> OnTimerActiveChange;

		public event Action<TimerStatus> OnTimerTick;

		public event Action OnBlockInteractionChange;

		public event Action OnBlockActionChange;

		public Tesla(ModuleDatas.Tesla data, ModuleConfigs.Tesla config)
			: base(data, config)
		{
			filterNode = (data.Activated ? new FilterNode<GIModuleType>(FilterType.Black, default(GIModuleType)) : FilterNode<GIModuleType>.Empty);
			actionsFilter = (base.Data.Activated ? new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Tesla) : new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Sell, GIModuleType.Tesla));
			speedUpSourceInfo = new SpeedUpSourceInfo("Tesla", base.Config.Multiplier, RefDateTime.Default);
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			if (base.Data.Activated)
			{
				CreateEffect();
			}
		}

		public void StartTimer()
		{
			lifeTimer = new TweenTimer(base.Data.LifeTimer, AtTimerComplete, AtTimerTick);
		}

		public override void Kill()
		{
			UnityEngine.Object.Destroy(effect);
			lifeTimer?.Kill();
		}

		public void Activate()
		{
			if (base.Data.Activated)
			{
				Debug.LogError("Tesla Already activated");
				return;
			}
			base.Data.Activated = true;
			base.Data.LifeTimer = new RefSkipableTimer(base.Config.LifeTime);
			speedUpSourceInfo.EndTime = new RefDateTime(TimeMaster.Now.AddSeconds(base.Config.LifeTime));
			StartTimer();
			CreateEffect();
			this.OnTimerActiveChange?.Invoke(obj: true);
			filterNode = new FilterNode<GIModuleType>(FilterType.Black, default(GIModuleType));
			actionsFilter = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Tesla);
			this.OnBlockInteractionChange?.Invoke();
			this.OnBlockActionChange?.Invoke();
		}

		private void CreateEffect()
		{
			effect = Controller<TeslaController>.Instance.CreateTelaEffect();
			base.Parent.AddChild(effect.transform, atCentre: true);
		}

		private void AtTimerComplete()
		{
			this.OnTimerComplete?.Invoke(this);
		}

		private void AtTimerTick(TimerStatus status)
		{
			this.OnTimerTick?.Invoke(status);
		}

		string IBlockModulesInteractionReasonable.GetReasonForCase(GIModuleType GIModuleType)
		{
			if (GIModuleType == GIModuleType.Merge)
			{
				return "item_activated";
			}
			return null;
		}
	}

	public class Stack : Casted<ModuleDatas.Stack, ModuleConfigs.Stack>, IBlockModulesInteraction, IBlockModulesAction
	{
		private RecipeModel activeRecipe;

		private List<RecipeModel> filterRecipes = new List<RecipeModel>();

		private Dictionary<GIKey, int> nextPushItem = new Dictionary<GIKey, int>();

		private FilterNode<GIModuleType> interactionsFilter;

		private FilterNode<GIModuleType> actionsFilter;

		public LightningTweenBuilder LightningEffect { get; private set; }

		public List<GIKey> nextItems => nextPushItem.Keys.ToList();

		public List<RecipeModel> FilterRecipe => filterRecipes;

		FilterNode<GIModuleType> IBlockModulesInteraction.InteractionsFilter => interactionsFilter;

		FilterNode<GIModuleType> IBlockModulesAction.ActionsFilter => actionsFilter;

		public event Action OnBlockInteractionChange;

		public event Action OnBlockActionChange;

		public Stack(ModuleDatas.Stack mData, ModuleConfigs.Stack mConfig)
			: base(mData, mConfig)
		{
		}

		protected override void AtAttach()
		{
			base.AtAttach();
			ValidateAll();
		}

		public void AttachTweener(LightningTweenBuilder tb)
		{
			LightningEffect = tb;
			LightningEffect.transform.SetParent(base.Parent.transform);
			LightningEffect.transform.localPosition = Vector3.zero;
			LightningEffect.transform.SetScale(base.Parent.SizeMul);
			LightningEffect.BuildTween(base.Parent.IconRenderer.transform, withEnergy: true);
			ValidateEffect();
			if (!base.Parent.AllowInteraction(GIModuleType.Stack))
			{
				base.Parent.OnBlockInteractionChange += UnlockCallback;
			}
		}

		public bool IsPushItem(GameItem gameItem)
		{
			return nextPushItem.ContainsKey(gameItem.Key);
		}

		public bool TryPushItem(GameItem gameItem)
		{
			if (activeRecipe != null)
			{
				return false;
			}
			GIData data = gameItem.Data.Copy();
			if (nextPushItem.ContainsKey(data.Key))
			{
				WeightNode<GIData> weightNode = base.Data.Items.FirstOrDefault((WeightNode<GIData> item) => item.value.Key == data.Key);
				if (weightNode != null)
				{
					weightNode.weight += 1f;
				}
				else
				{
					base.Data.Items.Add(new WeightNode<GIData>(data, 1f));
				}
				ValidateAll();
				return true;
			}
			ValidateAll();
			return false;
		}

		public bool TryPopItem(out GIData data)
		{
			if (base.Data.Items.Any())
			{
				WeightNode<GIData> weightNode = base.Data.Items.Last();
				data = weightNode.value.Copy();
				if ((int)weightNode.Weight - 1 <= 0)
				{
					base.Data.Items.Remove(weightNode);
				}
				else
				{
					weightNode.weight -= 1f;
				}
				ValidateAll();
				return true;
			}
			ValidateAll();
			data = null;
			return true;
		}

		private void ValidateAll()
		{
			ValidateItemsPool();
			ValidateFilter();
			ValidateBlockLists();
			ValidateEffect();
		}

		public void Clear()
		{
			activeRecipe = null;
			filterRecipes.Clear();
			base.Data.Items.Clear();
			ValidateAll();
		}

		public bool IsFullStack()
		{
			return activeRecipe != null;
		}

		private void ValidateItemsPool()
		{
			base.Data.Items = base.Data.Items.OrderBy((WeightNode<GIData> item) => item.value.Key).ToList();
		}

		private void ValidateFilter()
		{
			List<RecipeModel> list = new List<RecipeModel>();
			activeRecipe = null;
			nextPushItem?.Clear();
			foreach (RecipeModel item in base.Config.ItemsPool)
			{
				if (CheckRecipe(item))
				{
					list.Add(item);
				}
			}
			filterRecipes = ((base.Data.Items.Count == 0) ? base.Config.ItemsPool : list);
			if (activeRecipe != null)
			{
				SendRecipeToMixer(activeRecipe);
			}
		}

		private void SendRecipeToMixer(RecipeModel recipe)
		{
			if (base.Config.IsSwapActiveModule && base.Config.SwapModuleType == GIModuleType.Mixer && base.Parent != null)
			{
				Mixer box = base.Parent.GetBox<Mixer>();
				if (box != null && IsFullStack() && (!box.Data.ActiveRecipeID.HasValue || box.Data.CurrentState == ModuleDatas.Mixer.StateMixer.Wait))
				{
					box.Data.ActiveRecipeID = recipe.ID;
					box.BeginDefaultMixing();
				}
			}
		}

		private bool CheckRecipe(RecipeModel recipe)
		{
			if (!base.Data.Items.Any())
			{
				foreach (WeightNode<GIData> item in recipe.Items)
				{
					if (nextPushItem.ContainsKey(item.value.Key))
					{
						nextPushItem[item.value.Key] = ((nextPushItem[item.value.Key] > (int)item.Weight) ? ((int)item.Weight) : nextPushItem[item.value.Key]);
					}
					else
					{
						nextPushItem.Add(item.value.Key, (int)item.Weight);
					}
				}
			}
			IOrderedEnumerable<WeightNode<GIData>> first = base.Data.Items.OrderBy((WeightNode<GIData> item) => item.value.Key);
			IOrderedEnumerable<WeightNode<GIData>> second = recipe.Items.OrderBy((WeightNode<GIData> item) => item.value.Key);
			if (first.SequenceEqual(second, new WeightNodeWithCompleteCoincidenceComparer()))
			{
				activeRecipe = recipe;
				return true;
			}
			if (!base.Data.Items.Intersect(recipe.Items, new WeightNodeWithPartialMatchComparer()).Any())
			{
				return false;
			}
			foreach (WeightNode<GIData> recipeItem in recipe.Items)
			{
				int num = 0;
				WeightNode<GIData> weightNode = base.Data.Items.Find((WeightNode<GIData> item) => item.value.Key == recipeItem.value.Key);
				num = ((weightNode != null) ? ((int)recipeItem.Weight - (int)weightNode.Weight) : ((int)recipeItem.Weight));
				if (num > 0 && !nextPushItem.TryAdd(recipeItem.value.Key, num))
				{
					nextPushItem[recipeItem.value.Key] = ((nextPushItem[recipeItem.value.Key] > num) ? num : nextPushItem[recipeItem.value.Key]);
				}
			}
			return true;
		}

		private void ValidateBlockLists()
		{
			if (base.Config.IsSwapActiveModule)
			{
				FilterNode<GIModuleType> empty = FilterNode<GIModuleType>.Empty;
				FilterNode<GIModuleType> empty2 = FilterNode<GIModuleType>.Empty;
				if (IsFullStack())
				{
					empty = new FilterNode<GIModuleType>(FilterType.White, base.Config.SwapModuleType, GIModuleType.Merge);
					empty2 = new FilterNode<GIModuleType>(FilterType.White, base.Config.SwapModuleType, GIModuleType.Merge);
				}
				else if (base.Data.Items.Count == 0)
				{
					empty = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Stack, GIModuleType.Merge, GIModuleType.Sell);
					empty2 = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Stack, GIModuleType.Merge, GIModuleType.Sell);
				}
				else
				{
					empty = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Stack, GIModuleType.Merge);
					empty2 = new FilterNode<GIModuleType>(FilterType.White, GIModuleType.Stack, GIModuleType.Merge);
				}
				if (empty != actionsFilter)
				{
					actionsFilter = empty;
					this.OnBlockActionChange?.Invoke();
				}
				if (empty2 != interactionsFilter)
				{
					interactionsFilter = empty2;
					this.OnBlockInteractionChange?.Invoke();
				}
			}
		}

		public override void Kill()
		{
			LightningEffect?.Kill();
		}

		public void ValidateEffect()
		{
			bool flag = false;
			if (base.Parent.HasBox(GIModuleType.Bubble))
			{
				IBlockModulesInteraction box = base.Parent.GetBox<GreenT.HornyScapes.MergeCore.GameItemBox.Bubble>();
				flag = box != null && box is IBlockModulesAction blockModulesAction && box.InteractionsFilter.IsWhite && blockModulesAction.ActionsFilter.IsWhite;
			}
			base.Parent.SetActiveProgressContainer(!IsFullStack() && !base.Parent.IsLocked && !flag);
			base.Parent.ProgressSlider.value = Mathf.RoundToInt(base.Parent.ProgressSlider.maxValue * GetProgress());
			LightningEffect?.SetVisible(!IsFullStack() && !base.Parent.IsLocked && !flag);
		}

		private float GetProgress()
		{
			if (base.Data.Items.Count <= 0)
			{
				return 0f;
			}
			return base.Data.Items.Sum((WeightNode<GIData> item) => item.Weight) / FilterRecipe.First().Items.Sum((WeightNode<GIData> item) => item.Weight);
		}

		private void UnlockCallback(GameItem sender)
		{
			if (base.Parent.AllowInteraction(GIModuleType.Stack))
			{
				base.Parent.OnBlockInteractionChange -= UnlockCallback;
				ValidateEffect();
			}
		}
	}
}
