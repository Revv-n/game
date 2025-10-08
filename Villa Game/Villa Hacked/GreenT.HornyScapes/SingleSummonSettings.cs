using UnityEngine;

namespace GreenT.HornyScapes;

[CreateAssetMenu(fileName = "SingleSummonSettings", menuName = "StripClub/Shop/SingleSummonSettings")]
public class SingleSummonSettings : ScriptableObject
{
	[SerializeField]
	private Reward[] regularRewards;

	[SerializeField]
	private Reward[] rareRewards;

	[SerializeField]
	private Reward[] ultraRareRewards;

	public Reward[] RegularRewards => regularRewards;

	public Reward[] RareRewards => rareRewards;

	public Reward[] UltraRareRewards => ultraRareRewards;

	public int RegularRarity => 1;

	public int RareRarity => 2;

	public int UltraRareRarity => 3;
}
