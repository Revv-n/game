using System;
using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Tasks;
using GreenT.HornyScapes.Tasks.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class MiniEventButtonStrategy : BaseButtonStrategy
{
	public MiniEventTaskItemView MiniEventTaskItemView;

	private MiniEventTabRedirector _miniEventTabRedirection;

	private WindowArgumentApplier _windowArgumentApplier;

	[Inject]
	private void SetArgumentApplier(WindowArgumentApplier windowArgumentApplier)
	{
		_windowArgumentApplier = windowArgumentApplier;
	}

	public void SetClaimReward(Action claimReward)
	{
		_claimReward = claimReward;
		_isClaimRewardState = true;
	}

	protected override void SetRewardState()
	{
		MiniEventTaskItemView.SetRewardState();
	}

	protected override void HasStrategy(ActionButtonType actionButtonType)
	{
		base.HasStrategy(actionButtonType);
		if (!hasStrategy)
		{
			hasStrategy = buttonStrategyProvider.TryGetStrategy(source.Goal.ActionButtonType, out _miniEventTabRedirection);
		}
	}

	protected override void TransitionToWindow()
	{
		if (WindowOpener != null)
		{
			_windowArgumentApplier.SetArgs(source.Goal, WindowOpener.Windows);
			base.TransitionToWindow();
		}
		if (_miniEventTabRedirection != null && MiniEventTaskItemView.Source.Goal.Objectives.First() is ConcreteRouletteObjective concreteRouletteObjective)
		{
			_miniEventTabRedirection.RedirectRoulette(concreteRouletteObjective.RouletteId);
		}
	}
}
