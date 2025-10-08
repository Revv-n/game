using UnityEngine;

namespace GreenT.HornyScapes.Sounds;

[CreateAssetMenu(fileName = "TabSound", menuName = "GreenT/Sound/Tab")]
public class TabSoundSO : ScriptableObject
{
	[SerializeField]
	private AudioClip tabChoice;

	public AudioClip TabChoice => tabChoice;
}
