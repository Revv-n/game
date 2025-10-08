using GreenT.HornyScapes.Tasks.UI;
using GreenT.HornyScapes.UI;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventBaseTaskStateView : TaskViewState
{
	[SerializeField]
	protected int _buttonState;

	[SerializeField]
	protected int _buttonTextState;

	[SerializeField]
	protected MiniEventButtonStrategy _buttonStrategy;

	[SerializeField]
	protected AdvancedButtonSpriteStates _buttonSpriteStates;

	[SerializeField]
	protected TextMeshProValueStates _textMeshProValueStates;
}
