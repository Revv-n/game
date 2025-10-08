using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StripClub.Model;

[Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "StripClub/Skill/General", order = 0)]
public class SkillItemInfo : NamedScriptableItemInfo
{
	public enum Branch
	{
		Clicker,
		Idle,
		Energy
	}

	private const string prefix = "content.items.skill.";

	[field: SerializeField]
	public Branch Type { get; private set; }

	[field: SerializeField]
	public List<SkillItemInfo> Blockers { get; private set; }

	protected override string GetKey()
	{
		return "content.items.skill." + key;
	}

	public override void OnValidate()
	{
		base.OnValidate();
		if (Blockers.Any((SkillItemInfo blocker) => blocker.Equals(this)))
		{
			Blockers.Remove(this);
			Debug.LogError("Skill can't block himself. Error in file:" + base.name);
		}
	}
}
