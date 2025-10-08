using System.Collections.Generic;
using Merge;

namespace GreenT.HornyScapes.GameItems;

public class RecipeModel
{
	public int ID { get; }

	public List<WeightNode<GIData>> Items { get; }

	public List<WeightNode<GIData>> Result { get; }

	public int OutCount { get; }

	public int Time { get; }

	public float SecondPrice { get; }

	public RecipeModel(int id, List<WeightNode<GIData>> items, List<WeightNode<GIData>> result, int outCount, int time, float secondPrice)
	{
		ID = id;
		Items = items;
		Result = result;
		OutCount = outCount;
		Time = time;
		SecondPrice = secondPrice;
	}
}
