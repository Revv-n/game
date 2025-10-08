using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskToggleBlock : MonoBehaviour
{
	[SerializeField]
	private Toggle[] toggles;

	public void SetInteractible(bool value)
	{
		Toggle[] array = toggles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].interactable = value;
		}
	}
}
