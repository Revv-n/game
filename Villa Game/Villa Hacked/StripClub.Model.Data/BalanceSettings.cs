using GreenT.HornyScapes;
using UnityEngine;

namespace StripClub.Model.Data;

[CreateAssetMenu(fileName = "PlayerTemplate", menuName = "StripClub/Player/Create Player Template")]
public class BalanceSettings : ScriptableObject
{
	public SimpleCurrencyDictionary CurrencyDict;

	[SerializeField]
	public RestorableParameter ReplyEnergy;

	[SerializeField]
	public PlayerExperience PlayerExperience;

	[field: SerializeField]
	public int Level { get; private set; } = 1;

}
