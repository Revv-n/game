using GreenT.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

[RequireComponent(typeof(Window))]
public class SwitchStateWindowAdapter : MonoBehaviour
{
	[SerializeField]
	private Window window;

	public void SwitchState(bool isOpen)
	{
		if (isOpen)
		{
			window.Open();
		}
		else
		{
			window.Close();
		}
	}

	private void OnValidate()
	{
		if (!window)
		{
			window = GetComponent<Window>();
		}
	}
}
