using StripClub.Model.Character;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Models;

[CreateAssetMenu(fileName = "DateStories", menuName = "GreenT/HornyScapes/Date/DateStories")]
public class DateCharacterStories : CharacterStories
{
	private const string BundleName = "date/{0}/story/{1}";

	[SerializeField]
	private int _dateId;
}
