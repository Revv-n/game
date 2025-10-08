using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GreenT.HornyScapes.Relationships;

[Serializable]
public class RelationshipVisualDataGroup
{
	[SerializeField]
	public int status;

	[SerializeField]
	public Color color;

	[SerializeField]
	public List<RelationshipVisualDataEntry> entries;

	public RelationshipVisualDataEntry GetEntry(int count)
	{
		return entries.FirstOrDefault((RelationshipVisualDataEntry entry) => entry.count == count);
	}
}
