using System.Collections.Generic;
using Merge;
using Merge.Core.Balance;
using Zenject;

namespace GreenT.HornyScapes.GameItems;

public class RecipeFactory : IFactory<RecipeMapper, RecipeModel>, IFactory
{
	public RecipeModel Create(RecipeMapper mapper)
	{
		List<WeightNode<GIData>> items = ParsingUtils.ParseGIDataWeightList(mapper.items);
		List<WeightNode<GIData>> result = ParsingUtils.ParseGIDataWeightList(mapper.result);
		return new RecipeModel(mapper.uniq_id, items, result, mapper.out_count, mapper.time, mapper.second_price);
	}
}
