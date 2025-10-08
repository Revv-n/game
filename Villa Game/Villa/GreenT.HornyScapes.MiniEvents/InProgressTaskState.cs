using System.Linq;
using GreenT.HornyScapes.Tasks;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class InProgressTaskState : MiniEventBaseTaskStateView
{
	private readonly int CLAIM_TEXT_STATE = 2;

	public override void Enter()
	{
		_buttonSpriteStates.Set(_buttonState);
		_textMeshProValueStates.Set(_buttonTextState);
		_buttonStrategy.SetState(source.ReadyToComplete);
		bool flag = source.Goal.Objectives.Any((IObjective objective) => objective is CurrencyObjective && (objective as CurrencyObjective).CurrencyType == CurrencyType.MiniEvent);
		_textMeshProValueStates.Set(flag ? CLAIM_TEXT_STATE : _buttonTextState);
	}
}
