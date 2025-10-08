using System;
using System.Collections.Generic;
using System.Linq;

public static class List
{
	public static List<T> FilterOutBlack<T>(IList<T> toFilter, IList<T> blockList)
	{
		return FilterOut(toFilter, FilterType.Black, blockList);
	}

	public static List<T> FilterOutBlack<T>(IList<T> toFilter, params T[] blockList)
	{
		return FilterOutBlack(toFilter, (IList<T>)blockList);
	}

	public static List<T> FilterOutWhite<T>(IList<T> toFilter, IList<T> blockList)
	{
		return FilterOut(toFilter, FilterType.White, blockList);
	}

	public static List<T> FilterOutWhite<T>(IList<T> toFilter, params T[] blockList)
	{
		return FilterOutWhite(toFilter, (IList<T>)blockList);
	}

	public static List<T> FilterOut<T>(IList<T> toFilter, FilterType blockType, params T[] blockList)
	{
		return FilterOut(toFilter, blockType, (IList<T>)blockList);
	}

	public static List<T> FilterOut<T>(IList<T> toFilter, params FilterNode<T>[] nodes)
	{
		return FilterOut(toFilter, (IList<FilterNode<T>>)nodes);
	}

	public static List<T> FilterOut<T>(IList<T> toFilter, FilterType blockType, IList<T> blockList)
	{
		if (toFilter == null)
		{
			return null;
		}
		if (toFilter.Count == 0 || blockList == null || blockList.Count == 0)
		{
			return toFilter.ToList();
		}
		List<T> list = new List<T>();
		bool isWhite = blockType == FilterType.White;
		foreach (T item in toFilter)
		{
			if (blockList.Any((T x) => x.Equals(item) == isWhite))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<T> FilterOut<T>(IList<T> toFilter, IList<FilterNode<T>> nodes)
	{
		if (toFilter == null)
		{
			return null;
		}
		if (toFilter.Count == 0 || nodes == null || nodes.Count == 0)
		{
			return toFilter.ToList();
		}
		List<T> list = new List<T>();
		HashSet<T> hashSet = new HashSet<T>();
		HashSet<T> hashSet2 = new HashSet<T>();
		foreach (FilterNode<T> node in nodes)
		{
			if (node.IsWhite)
			{
				T[] list2 = node.list;
				foreach (T item2 in list2)
				{
					hashSet.Add(item2);
				}
			}
			if (node.IsBlack)
			{
				T[] list2 = node.list;
				foreach (T item3 in list2)
				{
					hashSet2.Add(item3);
				}
			}
		}
		foreach (T item in toFilter)
		{
			if (hashSet.Any((T x) => x.Equals(item)) && !hashSet2.Any((T x) => x.Equals(item)))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<T> FilterOut<T, D>(IList<T> toFilter, IList<FilterNode<D>> nodes, Func<T, D, bool> comparer)
	{
		if (toFilter == null)
		{
			return null;
		}
		if (toFilter.Count == 0 || nodes == null || nodes.Count == 0)
		{
			return toFilter.ToList();
		}
		List<T> list = new List<T>();
		HashSet<D> hashSet = new HashSet<D>();
		HashSet<D> hashSet2 = new HashSet<D>();
		foreach (FilterNode<D> node in nodes)
		{
			if (node.IsWhite)
			{
				D[] list2 = node.list;
				foreach (D item2 in list2)
				{
					hashSet.Add(item2);
				}
			}
			if (node.IsBlack)
			{
				D[] list2 = node.list;
				foreach (D item3 in list2)
				{
					hashSet2.Add(item3);
				}
			}
		}
		foreach (T item in toFilter)
		{
			if (hashSet.Any((D x) => comparer(item, x)) && !hashSet2.Any((D x) => comparer(item, x)))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
