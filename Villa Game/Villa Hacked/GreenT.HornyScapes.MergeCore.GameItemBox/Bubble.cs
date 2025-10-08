using System;
using DG.Tweening;
using Merge;
using Merge.MotionDesign;

namespace GreenT.HornyScapes.MergeCore.GameItemBox;

public class Bubble : GIBox.Casted<ModuleDatas.Bubble, ModuleConfigs.Bubble>, IControlClocks, IActionModule, IBlockModulesAction, IBlockModulesInteraction
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
