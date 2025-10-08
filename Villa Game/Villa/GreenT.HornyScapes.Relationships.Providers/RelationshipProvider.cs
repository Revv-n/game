using System.Linq;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Relationships.Providers;

public class RelationshipProvider : SimpleManager<Relationship>
{
	public Relationship Get(int id)
	{
		return Collection.FirstOrDefault((Relationship x) => x.ID == id);
	}

	public bool TryGet(int id, out Relationship relationship)
	{
		relationship = Collection.FirstOrDefault((Relationship x) => x.ID == id);
		return relationship != null;
	}
}
