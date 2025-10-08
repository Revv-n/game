using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes;

public class OnlyOneStatable : StatableComponentArray<GameObject>
{
	public override void Set(int stateNumber)
	{
		for (int i = 0; i < states.Length; i++)
		{
			states[i].SetActive(i == stateNumber);
		}
	}
}
