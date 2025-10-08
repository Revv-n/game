using System.Linq;
using GreenT.HornyScapes.Presents.Models;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.Presents;

public class PresentsManager : SimpleManager<Present>
{
	public Present Get(string id)
	{
		return Collection.FirstOrDefault((Present present) => present.Id == id);
	}

	public bool TryGet(string id, out Present present)
	{
		present = Get(id);
		return present != null;
	}
}
