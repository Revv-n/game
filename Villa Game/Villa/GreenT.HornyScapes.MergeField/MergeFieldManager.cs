using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes.MergeField;

public class MergeFieldManager : SimpleManager<MergeFieldMapper>
{
	public MergeFieldMapper GetDefault()
	{
		return collection.FirstOrDefault((MergeFieldMapper item) => item.bundle == "Main");
	}
}
