using GreenT.HornyScapes.Sounds;
using UnityEngine;

public class ScaleButtonSettings : MonoBehaviour
{
	public float scaleCoef = 1.05f;

	public float duration = 0.1f;

	public ButtonSoundSO Click;

	public ButtonSoundSO Hover;

	private void OnValidate()
	{
		if (Click == null)
		{
			Debug.LogWarning("Empty click sound", this);
		}
		if (Hover == null)
		{
			Debug.LogWarning("Empty hover sound", this);
		}
	}
}
