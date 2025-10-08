using UnityEngine;

namespace GreenT.HornyScapes.Dates.Providers;

[CreateAssetMenu(fileName = "DatesSoundsProvider", menuName = "GreenT/HornyScapes/Date/Sounds/DatesSoundsProvider")]
public class DatesSoundsProvider : ScriptableObject
{
	[Header("ID звука => звук")]
	[SerializeField]
	public DateSoundsDictionary BackgroundSounds;

	[SerializeField]
	public DateSoundsPresetDictionary SoundEffects;
}
