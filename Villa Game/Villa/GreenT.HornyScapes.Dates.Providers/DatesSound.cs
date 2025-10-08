using UnityEngine;

namespace GreenT.HornyScapes.Dates.Providers;

[CreateAssetMenu(fileName = "DatesSound", menuName = "GreenT/HornyScapes/Date/Sounds/DatesSound")]
public class DatesSound : ScriptableObject
{
	[SerializeField]
	private AudioClip _sound;

	[SerializeField]
	private float _cooldown;

	[Header("Assets/Recources/[SoundPath]")]
	[SerializeField]
	private string _soundPath;

	public float Cooldown => _cooldown;

	public AudioClip Sound => _sound;
}
