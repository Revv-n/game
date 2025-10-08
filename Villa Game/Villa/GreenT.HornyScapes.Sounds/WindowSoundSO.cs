using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "WindowSound", menuName = "GreenT/Sound/Window")]
public class WindowSoundSO : ScriptableObject
{
	[SerializeField]
	private AudioClip open;

	[SerializeField]
	private AudioClip close;

	public AudioClip Open => open;

	public AudioClip Close => close;
}
