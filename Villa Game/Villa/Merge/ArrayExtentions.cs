using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Merge;

public static class ArrayExtentions
{
	public static bool IsNullOrEmpty(this ICollection collection)
	{
		if (collection != null)
		{
			return collection.Count == 0;
		}
		return true;
	}

	public static bool TryGetElement<T>(this ICollection<T> collection, int index, out T element)
	{
		element = default(T);
		if (!((ICollection)collection).IsNullOrEmpty() && collection.ValidateIndex(index))
		{
			element = collection.ElementAt(index);
			return true;
		}
		return false;
	}

	public static bool ValidateIndex<T>(this ICollection<T> array, int index)
	{
		if (array.Count > index)
		{
			return index >= 0;
		}
		return false;
	}

	public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
	{
		return (from x in Enumerable.Range(0, matrix.GetLength(0))
			select matrix[x, columnNumber]).ToArray();
	}

	public static T[] GetRow<T>(this T[,] matrix, int rowNumber)
	{
		return (from x in Enumerable.Range(0, matrix.GetLength(1))
			select matrix[rowNumber, x]).ToArray();
	}
}
