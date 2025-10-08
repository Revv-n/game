using GreenT.HornyScapes.Dates.Views;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Models;

[CreateAssetMenu(fileName = "DateBackgroundData", menuName = "GreenT/HornyScapes/Date/DateBackgroundData")]
public class DateBackgroundData : ScriptableObject
{
	[field: SerializeField]
	public string ID { get; private set; }

	[field: SerializeField]
	public Sprite StaticBackground { get; private set; }

	[field: SerializeField]
	public AnimationDateView AnimatedBackground { get; private set; }
}
