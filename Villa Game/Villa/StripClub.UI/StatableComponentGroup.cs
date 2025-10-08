using UnityEngine;

namespace StripClub.UI;

public class StatableComponentGroup : StatableComponent
{
	[SerializeField]
	private StatableComponent[] statableComponents;

	public override void Set(int stateNumber)
	{
		StatableComponent[] array = statableComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(stateNumber);
		}
	}
}
