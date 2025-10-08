using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.GameItems;

public class RecipeStructureInitializer : StructureInitializerViaArray<RecipeMapper, RecipeModel>
{
	public RecipeStructureInitializer(IManager<RecipeModel> manager, IFactory<RecipeMapper, RecipeModel> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
