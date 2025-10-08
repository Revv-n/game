using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class VibrateButton : MonoBehaviour
{
	private const string VIBR = "Vibration";

	private void Start()
	{
		GetComponent<Button>().AddClickCallback(delegate
		{
			VibrationOnClick();
		});
	}

	private void VibrationOnClick()
	{
	}
}
