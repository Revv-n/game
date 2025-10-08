using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.MergeField;
using GreenT.Types;
using Merge;
using Merge.Core.Balance;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class MergeFieldFactory : IFactory<MergeFieldMapper, string, MergeField>, IFactory
{
	private readonly ISaver saver;

	public MergeFieldFactory(ISaver saver)
	{
		this.saver = saver;
	}

	public MergeField Create(MergeFieldMapper mapper, string saveKey)
	{
		List<GIData> items = ParseTiles(mapper.items);
		ContentType type = ((!(mapper.bundle == "Main")) ? ContentType.Event : ContentType.Main);
		return Create(items, type, saveKey);
	}

	private static List<GIData> ParseTiles(string[] items)
	{
		List<GIData> list = new List<GIData>();
		foreach (IGrouping<int, (Point, string)> item3 in from item in items
			select item.Split("?", StringSplitOptions.None) into expression
			select (expression[0], expression[expression.Length - 1]) into tuple
			select (tuple.Item1.Split(":", StringSplitOptions.None), tuple.Item2) into tuple
			select (new Point(int.Parse(tuple.Item1[0]), int.Parse(tuple.Item1[1])), tuple.Item2) into tile
			group tile by tile.Item1.Y into item
			orderby item.Key descending
			select item)
		{
			foreach (var item4 in item3)
			{
				GIData gIData = ParsingUtils.ParseGIData(item4.Item2);
				if (!gIData.Empty)
				{
					Point item2 = item4.Item1;
					int x = item2.X;
					item2 = item4.Item1;
					gIData.Coordinates = new Point(x, item2.Y);
					list.Add(gIData);
				}
			}
		}
		return list;
	}

	public MergeField Create(List<GIData> items, ContentType type, string saveKey)
	{
		GeneralData data = new GeneralData(items, type, saveKey);
		return CreateMergeField(data);
	}

	private MergeField CreateMergeField(GeneralData data)
	{
		MergeField mergeField = new MergeField(data, new Point(10, 6));
		saver.Add(mergeField.Data);
		return mergeField;
	}
}
