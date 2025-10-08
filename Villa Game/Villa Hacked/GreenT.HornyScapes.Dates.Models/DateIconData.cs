using UnityEngine;

namespace GreenT.HornyScapes.Dates.Models;

[CreateAssetMenu(fileName = "DateIconData", menuName = "GreenT/HornyScapes/Date/DateIconData")]
public class DateIconData : ScriptableObject
{
	[field: SerializeField]
	public int ID { get; private set; }

	[field: SerializeField]
	public Sprite Icon { get; private set; }
}
