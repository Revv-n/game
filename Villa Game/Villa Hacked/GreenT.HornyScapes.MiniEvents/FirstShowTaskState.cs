using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Tasks;
using StripClub.Model;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class FirstShowTaskState : MiniEventBaseTaskStateView
{
	[SerializeField]
	private RectTransformAnimation _showAnimation;

	[SerializeField]
	private RectTransformAnimation _hideAnimation;

	private readonly int CLAIM_TEXT_STATE = 2;

	public override void Enter()
	{
		base.Enter();
		bool flag = source.Goal.Objectives.Any((IObjective objective) => objective is CurrencyObjective && (objective as CurrencyObjective).CurrencyType == CurrencyType.MiniEvent);
		_textMeshProValueStates.Set(flag ? CLAIM_TEXT_STATE : _buttonTextState);
		_hideAnimation.Stop();
		_showAnimation.Play();
	}
}
