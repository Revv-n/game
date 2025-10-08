using UnityEngine;

namespace GreenT.HornyScapes.Dates.Providers;

[CreateAssetMenu(fileName = "DatesSoundsPreset", menuName = "GreenT/HornyScapes/Date/Sounds/DatesSoundsPreset")]
public class DatesSoundsPreset : ScriptableObject
{
	[SerializeField]
	private DatesSoundtracksInfoDictionary _soundtracks;

	public DatesSoundtracksInfoDictionary Soundtracks => _soundtracks;
}
