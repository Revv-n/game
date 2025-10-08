using UnityEngine;

namespace GreenT.HornyScapes.Dates.Providers;

[CreateAssetMenu(fileName = "DatesSoundtracks", menuName = "GreenT/HornyScapes/Date/Sounds/DatesSoundtracks")]
public class DatesSoundtracks : ScriptableObject
{
	[SerializeField]
	private bool _isRandom;

	[SerializeField]
	private bool _isRepeat;

	[SerializeField]
	private float _startCooldown;

	[SerializeField]
	private DatesSound[] _dateSounds;

	public bool IsRandom => _isRandom;

	public bool IsRepeat => _isRepeat;

	public float StartCooldown => _startCooldown;

	public DatesSound[] DateSounds => _dateSounds;
}
