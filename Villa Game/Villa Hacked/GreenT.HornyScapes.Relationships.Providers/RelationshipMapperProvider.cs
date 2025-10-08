using System;
using System.Linq;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Relationships.Providers;

public class RelationshipMapperProvider : SimpleManager<RelationshipMapper>
{
	public RelationshipMapper Get(int id)
	{
		RelationshipMapper relationshipMapper = collection.FirstOrDefault((RelationshipMapper x) => x.id_relationship == id);
		if (relationshipMapper == null)
		{
			new NullReferenceException($"No relationship with id: {id}").LogException();
			return null;
		}
		return relationshipMapper;
	}
}
