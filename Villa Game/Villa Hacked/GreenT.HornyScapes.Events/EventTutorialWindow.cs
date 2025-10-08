using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventTutorialWindow : TutorialWindow
{
	[SerializeField]
	private EventTutorialStrategyResolver _strategyResolver;

	public override void Open()
	{
		_strategyResolver.Show();
		base.Open();
	}
}
