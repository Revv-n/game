using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "MultiplierSkill", menuName = "StripClub/Skill/Multiplier", order = 1)]
public class MultiplierSkillItemInfo : SkillItemInfo
{
	[SerializeField]
	private float multiplierPerLevel = 1f;

	public float MultiplierPerLevel => multiplierPerLevel;
}
