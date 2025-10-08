using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Extensions;
using Merge.MotionDesign.Tweeners;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class GameItem : MonoBehaviour, IHasCoordinates
{
	private readonly Subject<GameItem> _onRemovingObservable = new Subject<GameItem>();

	[SerializeField]
	private SpriteRenderer sr;

	[SerializeField]
	private ItemClock clock;

	[SerializeField]
	private Slider progress;

	[SerializeField]
	private GameObject progressContainer;

	[SerializeField]
	private Transform sizeFitter;

	[SerializeField]
	private GameObject taskMark;

	[SerializeField]
	private float lerpForce = 3f;

	private List<IControlClocks> subscribedClocks = new List<IControlClocks>();

	private Tween clockSpeedUpTween;

	private int taskMarkRef;

	public int ExtraMarkRef;

	public bool HasLightSubTile;

	[SerializeField]
	private GameItemMergePointsCase battlePassCase;

	private List<Tween> outerTweens = new List<Tween>();

	public const int FlyOrderBonus = 100;

	private GameObject shineMask;

	[HideInInspector]
	public LightningSystem GrayScaleSystem;

	public MaterialPropertyBlock PropBlock;

	private Tween grayScaleTween;

	public IObservable<GameItem> OnRemovingObservable => _onRemovingObservable;

	public List<GIBox.Base> Boxes { get; private set; } = new List<GIBox.Base>();


	public GIConfig Config { get; private set; }

	public GIData Data { get; private set; }

	public Tween MoveTween { get; private set; }

	public Tween CreateTween { get; private set; }

	public Slider ProgressSlider => progress;

	public bool HasTaskMark => taskMark.activeSelf;

	public bool IsDraging { get; private set; }

	public Sprite Icon
	{
		get
		{
			return sr.sprite;
		}
		set
		{
			sr.sprite = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	public Point Coordinates
	{
		get
		{
			return Data.Coordinates;
		}
		set
		{
			Data.Coordinates = value;
		}
	}

	public bool IsAnimatingMotion => MoveTween != null;

	public SpriteRenderer IconRenderer => sr;

	public GameItemMergePointsCase MergePointsCase => battlePassCase;

	public GIKey Key => Config.Key;

	public float SizeMul => sizeFitter.localScale.x;

	public bool IsLocked => Data.HasModule(GIModuleType.Locked);

	public bool IsHardLocked
	{
		get
		{
			if (Data.HasModule(GIModuleType.Locked))
			{
				return Data.GetModule<ModuleDatas.Locked>().BlocksMerge;
			}
			return false;
		}
	}

	public bool IsBubbled => Data.HasModule(GIModuleType.Bubble);

	public bool IsSpawner
	{
		get
		{
			if (!HasBox(GIModuleType.AutoSpawn))
			{
				return HasBox(GIModuleType.ClickSpawn);
			}
			return true;
		}
	}

	public bool IsMixer => HasBox(GIModuleType.Mixer);

	public bool IsEmptySpawner
	{
		get
		{
			TryGetBox<GIBox.ClickSpawn>(out var box);
			if (box == null)
			{
				return true;
			}
			return !box.CanSpawn;
		}
	}

	public bool IsTimerVisible
	{
		get
		{
			TryGetBox<GIBox.ClickSpawn>(out var box);
			TryGetBox<GIBox.AutoSpawn>(out var box2);
			if (box == null || !box.IsTimerVisible)
			{
				return box2?.IsTimerVisible ?? false;
			}
			return true;
		}
	}

	public bool AllowMotion
	{
		get
		{
			if (!IsAnimatingMotion)
			{
				return !Boxes.Any((GIBox.Base x) => x is IMotionBlocker && (x as IMotionBlocker).BlockMotion);
			}
			return false;
		}
	}

	public bool IsRemovingNow { get; private set; }

	public event Action<GameItem> OnRemoving;

	public event Action<GameItem, Point> OnMovedFrom;

	public event Action<GameItem> OnBecomeDrag;

	public event Action<GameItem> OnEndDrag;

	public event Action<GameItem, GIModuleType> OnModuleRemoved;

	public event Action<GameItem> OnBlockActionChange;

	public event Action<GameItem> OnBlockInteractionChange;

	public event Action<GameItem, bool> OnTimerActiveChange;

	public void SetActiveProgressContainer(bool active)
	{
		progressContainer.SetActive(active);
	}

	public void ChangeTaskMarkRef(int addCount)
	{
		taskMarkRef += addCount;
		SetTaskMark(taskMarkRef > 0);
		_ = taskMarkRef;
		_ = 0;
	}

	public void ChangeExtraMarkRef(int addCount)
	{
		ExtraMarkRef += addCount;
		SetTaskMark(ExtraMarkRef > 0);
		_ = ExtraMarkRef;
		_ = 0;
	}

	public bool TryGetLockedIcon(out Sprite icon)
	{
		if (TryGetBox<GIBox.Locked>(out var box))
		{
			icon = box.LockedIcon.sprite;
			return true;
		}
		icon = null;
		return false;
	}

	public bool AllowInteraction(GIModuleType type)
	{
		return !Boxes.Any((GIBox.Base x) => x is IBlockModulesInteraction && (x as IBlockModulesInteraction).InteractionsFilter.IsBlocks(type));
	}

	public bool AllowInteraction(GIModuleType type, out string message)
	{
		foreach (GIBox.Base box in Boxes)
		{
			if (box is IBlockModulesInteractionReasonable && (box as IBlockModulesInteractionReasonable).InteractionsFilter.IsBlocks(type))
			{
				message = (box as IBlockModulesInteractionReasonable).GetReasonForCase(type);
				return false;
			}
		}
		message = null;
		return true;
	}

	public void Init(GIData data, GIConfig config)
	{
		Config = config;
		Data = data;
	}

	public void Save()
	{
		Boxes.ForEach(delegate(GIBox.Base x)
		{
			x.Save();
		});
	}

	public void LerpTo(Vector3 position)
	{
		base.transform.position = Vector3.Lerp(base.transform.position, position, Time.deltaTime * lerpForce);
	}

	public void AddBox(GIBox.Base box)
	{
		box.Attach(this);
		if (box is IControlClocks controlClocks)
		{
			IControlClocks controlClocks2 = FindMaxClockPriority();
			if (controlClocks2 == null || controlClocks.ClockControlPriority > controlClocks2.ClockControlPriority)
			{
				if (controlClocks2 != null)
				{
					UnsubscribeClock(controlClocks2);
				}
				SubscribeClock(controlClocks);
				AtTimerActiveChange(controlClocks.IsTimerVisible);
			}
		}
		if (box is ISpeedUpReciver speedUpReciver)
		{
			speedUpReciver.OnSpeedUpChange += AtModuleSpeedUpChange;
			if (clockSpeedUpTween == null && speedUpReciver.SpeedUpMultiplyer > 1f)
			{
				StartClockSpeedUpTween();
			}
		}
		if (box is IBlockModulesAction blockModulesAction)
		{
			blockModulesAction.OnBlockActionChange += AtBlockActionChange;
		}
		if (box is IBlockModulesInteraction blockModulesInteraction)
		{
			blockModulesInteraction.OnBlockInteractionChange += AtBlockInteractionChange;
		}
		Boxes.Add(box);
	}

	public T GetBox<T>() where T : GIBox.Base
	{
		return Boxes.FirstOrDefault((GIBox.Base x) => x is T) as T;
	}

	public bool TryGetBox<T>(out T box) where T : GIBox.Base
	{
		box = Boxes.FirstOrDefault((GIBox.Base x) => x is T) as T;
		return box != null;
	}

	public bool HasBox(GIModuleType type)
	{
		return Boxes.Any((GIBox.Base x) => x.ModuleType == type);
	}

	public void BeginRemove()
	{
		IsRemovingNow = true;
		foreach (Tween outerTween in outerTweens)
		{
			outerTween?.Kill();
		}
		outerTweens.Clear();
		this.OnRemoving?.Invoke(this);
		_onRemovingObservable.OnNext(this);
		RemoveAllModules();
	}

	public void SetDragged(bool isDragged)
	{
		if (isDragged != IsDraging)
		{
			IsDraging = isDragged;
			if (isDragged)
			{
				this.OnBecomeDrag?.Invoke(this);
			}
			else
			{
				this.OnEndDrag?.Invoke(this);
			}
		}
	}

	public void SendActionMovedFrom(Point oldCoord)
	{
		this.OnMovedFrom?.Invoke(this, oldCoord);
	}

	public void AddChild(Transform child, bool atCentre = false, bool validateScale = true)
	{
		child.SetParent(sizeFitter);
		if (atCentre)
		{
			child.localPosition = Vector3.zero;
		}
		if (validateScale)
		{
			child.localScale = Vector3.one;
		}
	}

	public void RemoveModule(GIModuleType type)
	{
		GIBox.Base @base = Boxes.FirstOrDefault((GIBox.Base x) => x.ModuleType == type);
		if (@base != null)
		{
			@base.Kill();
			Boxes.Remove(@base);
		}
		for (int i = 0; i < Data.Modules.Count; i++)
		{
			if (Data.Modules[i].ModuleType == type)
			{
				Data.Modules.RemoveAt(i);
				break;
			}
		}
		if (@base is ISpeedUpReciver)
		{
			AtModuleSpeedUpChange(active: false);
		}
		this.OnModuleRemoved?.Invoke(this, type);
	}

	private void RemoveAllModules()
	{
		foreach (GIBox.Base box in Boxes)
		{
			box.Kill();
		}
		Boxes.Clear();
		Data.Modules.Clear();
	}

	public void AppendOuterTween(Tween tween)
	{
		outerTweens.Add(tween);
		Tween tween2 = tween;
		tween2.onComplete = (TweenCallback)Delegate.Combine(tween2.onComplete, (TweenCallback)delegate
		{
			outerTweens.Remove(tween);
		});
	}

	public void DoFlyTo(Vector3 position)
	{
		MoveTween = BuildBezierFlySmall(base.transform.position, position, 0.4f);
		Tween moveTween = MoveTween;
		moveTween.onComplete = (TweenCallback)Delegate.Combine(moveTween.onComplete, (TweenCallback)delegate
		{
			MoveTween = null;
		});
	}

	public Tween DoCreate()
	{
		base.transform.localScale = Vector3.zero;
		IconRenderer.SetAlpha(0f);
		Sequence sequence = DOTween.Sequence();
		sequence.Append(base.transform.DOScale(1.3f, 0.24f).SetEase(Ease.OutFlash));
		sequence.Join(IconRenderer.DOFade(1f, 0.24f));
		sequence.Append(base.transform.DOScale(1f, 0.15f).SetEase(Ease.InSine));
		CreateTween = sequence;
		Tween createTween = CreateTween;
		createTween.onComplete = (TweenCallback)Delegate.Combine(createTween.onComplete, (TweenCallback)delegate
		{
			CreateTween = null;
		});
		return CreateTween;
	}

	public void DoCreateFrom(Vector3 position)
	{
		Vector3 position2 = Position;
		base.transform.SetScale(0.4f);
		CreateTween = TweenerMaster.TweenerGIFly.CreateFlyTween(this, position, position2);
		Tween createTween = CreateTween;
		createTween.onComplete = (TweenCallback)Delegate.Combine(createTween.onComplete, (TweenCallback)delegate
		{
			CreateTween = null;
		});
	}

	private Tween BuildBezierFlySmall(Vector3 a, Vector3 b, float time)
	{
		float num = 1000f;
		float num2 = Mathf.Clamp((b - a).sqrMagnitude / num, 0.4f, 1.5f);
		base.transform.position = a;
		IconRenderer.sortingOrder += 100;
		Sequence sequence = DOTween.Sequence();
		float num3 = 0.7f;
		Sequence sequence2 = DOTween.Sequence();
		sequence2.Append(base.transform.DOScale(1f + 0.2f * num2, time * num3).SetEase(Ease.OutSine));
		sequence2.Append(base.transform.DOScale(1f, time - time * num3).SetEase(Ease.InSine));
		sequence.Append(EasyBezierTweener.DoBezier(base.transform, a, b, time, 0.1f, num3, 1));
		sequence.Join(sequence2);
		sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(AtComplete));
		return sequence;
		void AtComplete()
		{
			IconRenderer.sortingOrder -= 100;
		}
	}

	private void StartClockSpeedUpTween()
	{
		Sequence sequence = DOTween.Sequence();
		float num = 10f;
		float scaleTime = 1f;
		float scaleSize = 1.2f;
		clock.Root.SetScale(1f);
		clock.Root.localEulerAngles = new Vector3(0f, 0f, num);
		sequence.Append(GetScaleSeq());
		sequence.Join(clock.Root.transform.DORotate(new Vector3(0f, 0f, 0f - num), scaleTime * 2f).SetEase(Ease.InOutFlash));
		sequence.Append(GetScaleSeq());
		sequence.Join(clock.Root.transform.DORotate(new Vector3(0f, 0f, num), scaleTime * 2f).SetEase(Ease.InOutFlash));
		sequence.SetLoops(-1, LoopType.Restart);
		clockSpeedUpTween = sequence;
		Tween GetScaleSeq()
		{
			Sequence sequence2 = DOTween.Sequence();
			sequence2.Append(clock.Root.transform.DOScale(scaleSize, scaleTime).SetEase(Ease.InOutFlash));
			sequence2.Append(clock.Root.transform.DOScale(1f, scaleTime).SetEase(Ease.InOutFlash));
			return sequence2;
		}
	}

	private void AtModuleSpeedUpChange(bool active)
	{
		if (active != (clockSpeedUpTween != null))
		{
			if (!active)
			{
				clockSpeedUpTween?.Kill();
				clockSpeedUpTween = null;
				clock.Root.SetScale(1f);
				clock.Root.localEulerAngles = Vector3.zero;
			}
			else
			{
				StartClockSpeedUpTween();
			}
		}
	}

	private void AtBlockActionChange()
	{
		this.OnBlockActionChange?.Invoke(this);
	}

	private void AtBlockInteractionChange()
	{
		this.OnBlockInteractionChange?.Invoke(this);
	}

	private IControlClocks FindMaxClockPriority()
	{
		try
		{
			return Boxes.OfType<IControlClocks>().Aggregate((IControlClocks current, IControlClocks next) => (current.ClockControlPriority <= next.ClockControlPriority) ? next : current);
		}
		catch
		{
			return null;
		}
	}

	private void SubscribeClock(IControlClocks value)
	{
		value.OnTimerTick += AtTimerTick;
		value.OnTimerActiveChange += AtTimerActiveChange;
		subscribedClocks.Add(value);
	}

	private void UnsubscribeAllClock()
	{
		foreach (IControlClocks subscribedClock in subscribedClocks)
		{
			subscribedClock.OnTimerTick -= AtTimerTick;
			subscribedClock.OnTimerActiveChange -= AtTimerActiveChange;
		}
		subscribedClocks.Clear();
	}

	private void UnsubscribeClock(IControlClocks value)
	{
		value.OnTimerTick -= AtTimerTick;
		value.OnTimerActiveChange -= AtTimerActiveChange;
		subscribedClocks.Remove(value);
	}

	private void AtTimerTick(TimerStatus timerInfo)
	{
		clock.SetTimerInfo(timerInfo);
	}

	private void SetEnergyClock()
	{
		clock.SetEnergySprite();
	}

	private void SetLockClock()
	{
		clock.SetLockerSprite();
	}

	private void SetMixingClock()
	{
		clock.SetMixingSprite();
	}

	public void SetIconClock()
	{
		if (HasBox(GIModuleType.Mixer))
		{
			SetMixingClock();
		}
		else if (HasBox(GIModuleType.Chest))
		{
			SetLockClock();
		}
		else
		{
			SetEnergyClock();
		}
	}

	private void AtTimerActiveChange(bool active)
	{
		if (!IsBubbled && !IsLocked && clock.gameObject.activeSelf != active)
		{
			grayScaleTween?.Kill();
			if (active)
			{
				grayScaleTween = GrayScaleSystem.Apply(IconRenderer, PropBlock);
			}
			else
			{
				grayScaleTween = GrayScaleSystem.Undo(IconRenderer, PropBlock);
			}
			clock.SetActive(active);
			this.OnTimerActiveChange?.Invoke(this, active);
		}
	}

	private void SetTaskMark(bool active)
	{
		if (!(taskMark == null) && !taskMark.gameObject.IsDestroyed())
		{
			taskMark.SetActive(active);
		}
	}

	private void OnDestroy()
	{
		UnsubscribeAllClock();
		MoveTween?.Kill();
		CreateTween?.Kill();
		base.transform.DOKill();
		IconRenderer.DOKill();
		IconRenderer.transform.DOKill();
		foreach (GIBox.Base box in Boxes)
		{
			box.Kill();
		}
	}
}
