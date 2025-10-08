using System;
using System.Linq;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Relationships.Providers;

public class RelationshipRewardMapperProvider : SimpleManager<RelationshipRewardMapper>
{
	public RelationshipRewardMapper Get(int id)
	{
		RelationshipRewardMapper relationshipRewardMapper = collection.FirstOrDefault((RelationshipRewardMapper x) => x.id == id);
		if (relationshipRewardMapper == null)
		{
			new NullReferenceException($"No relationship reward with id: {id}").LogException();
			return null;
		}
		return relationshipRewardMapper;
	}
}
