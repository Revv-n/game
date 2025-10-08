using System;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships;

[Serializable]
public class RelationshipVisualDataEntry
{
	[SerializeField]
	public int count;

	[SerializeField]
	public TMP_SpriteAsset spriteAsset;
}
