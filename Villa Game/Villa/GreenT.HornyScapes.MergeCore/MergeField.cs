using System;
using System.Collections.Generic;
using GreenT.Types;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class MergeField
{
	public readonly Point Size;

	public readonly GeneralData Data;

	public FieldMonoMediator FieldMediator;

	public readonly int MaxCount;

	public bool Inited;

	public PointMatrix<GameItem> Field { get; protected set; }

	public ContentType Type => Data.Type;

	public bool IsFull => CurrentItemsCount == MaxCount;

	public int CurrentItemsCount { get; private set; }

	public MergeField(GeneralData data, Point size)
	{
		Data = data;
		Size = size;
		MaxCount = Size.X * Size.Y;
		Field = PointMatrix<GameItem>.CreateBySize(Size);
	}

	public void SetTransform(FieldMonoMediator fieldMediator)
	{
		FieldMediator = fieldMediator;
	}

	public void SetGameItem(Point coordinate, GameItem item)
	{
		Field[coordinate] = item;
		CurrentItemsCount++;
	}

	public void ResetField()
	{
		RemoveAllItems();
		Inited = false;
	}

	private void RemoveAllItems()
	{
		if (!Inited)
		{
			return;
		}
		foreach (GameItem @object in Field.Objects)
		{
			ForceDeleteItem(@object);
		}
	}

	private void ForceDeleteItem(GameItem item)
	{
		Field[item.Coordinates] = null;
		Data.GameItems.Remove(item.Data);
		item.BeginRemove();
		UnityEngine.Object.Destroy(item.gameObject);
		CurrentItemsCount--;
	}

	public void RemoveItem(GameItem item)
	{
		Field[item.Coordinates] = null;
		Data.GameItems.Remove(item.Data);
		CurrentItemsCount--;
	}

	public List<Point> GetEmptyTilesDonut(Point centre)
	{
		return GetTilesDonut(centre, (GameItem x) => x == null);
	}

	public List<Point> GetNotEmptyTilesDonut(Point centre)
	{
		return GetTilesDonut(centre, (GameItem x) => x != null);
	}

	public List<Point> GetTilesDonut(Point centre, Func<GameItem, bool> predicate)
	{
		List<Point> list = new List<Point>();
		for (int i = centre.Y - 1; i < centre.Y + 2; i++)
		{
			for (int j = centre.X - 1; j < centre.X + 2; j++)
			{
				if (Field.Contains(j, i) && !centre.Is(j, i) && (predicate == null || predicate(Field[j, i])))
				{
					list.Add(new Point(j, i));
				}
			}
		}
		return list;
	}

	public void InvokeSaveAllItems()
	{
		foreach (GameItem item in Field)
		{
			if (!(item == null))
			{
				item.Save();
			}
		}
	}
}
