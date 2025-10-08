using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships;

[CreateAssetMenu(fileName = "RelationshipVisualData", menuName = "GreenT/HornyScapes/Relationships/VisualData", order = 0)]
public class RelationshipVisualData : ScriptableObject
{
	[SerializeField]
	public RelationshipSpriteType spriteType;

	[SerializeField]
	public TMP_FontAsset font;

	[SerializeField]
	public Material materialPreset;

	[SerializeField]
	public float fontSize;

	[SerializeField]
	public List<RelationshipVisualDataGroup> groups;

	public string GetStatusText(int status, int count, string text)
	{
		return $"<font=\"{font.name}\"><material=\"{materialPreset.name}\"><size={fontSize}><color=#{ColorUtility.ToHtmlStringRGBA(GetColor(status))}>{$"<nobr>{text} {GetSprite(status, count)}</nobr>"}</color></size></material></font>";
	}

	private string GetSprite(int status, int count)
	{
		RelationshipVisualDataEntry relationshipVisualDataEntry = GetGroup(status)?.GetEntry(count);
		if (relationshipVisualDataEntry == null)
		{
			return string.Empty;
		}
		return $"<sprite=\"{relationshipVisualDataEntry.spriteAsset.name}\" index={(int)spriteType}>";
	}

	private Color GetColor(int status)
	{
		return GetGroup(status)?.color ?? Color.white;
	}

	private RelationshipVisualDataGroup GetGroup(int status)
	{
		return groups.FirstOrDefault((RelationshipVisualDataGroup group) => group.status == status);
	}
}
