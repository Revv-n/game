using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Merge;

[Serializable]
public class PointMatrix<T> : IEnumerable<T>, IEnumerable where T : IHasCoordinates
{
	[Serializable]
	public class Row : IEnumerable<T>, IEnumerable
	{
		public T[] items;

		public Row(int size)
		{
			items = new T[size];
		}

		public Row(T[] items)
		{
			this.items = items;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}
	}

	public class PointMatrixEnumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private Row[] rows;

		private int x = -1;

		private int y;

		public T Current
		{
			get
			{
				if (x == -1 || y >= rows.Length || x >= rows[0].items.Length)
				{
					throw new InvalidOperationException();
				}
				return rows[y].items[x];
			}
		}

		object IEnumerator.Current => Current;

		public PointMatrixEnumerator(Row[] rows)
		{
			this.rows = rows;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (x < rows[y].items.Length - 1)
			{
				x++;
				return true;
			}
			if (y < rows.Length - 1)
			{
				x = 0;
				y++;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			x = -1;
			y = 0;
		}
	}

	[SerializeField]
	private Row[] rows;

	public IEnumerable<T> Objects => this.Where((T _cell) => _cell != null);

	public HashSet<T> HashSetObjects => Objects.ToHashSet();

	public Point Position { get; private set; }

	public Point Size { get; private set; }

	public int X => Position.X;

	public int Y => Position.Y;

	public int Width => Size.X;

	public int Height => Size.Y;

	public Point Max => Position + Size - Point.One;

	public Point Min => Position;

	public T this[int x, int y]
	{
		get
		{
			return GetValue(x, y);
		}
		set
		{
			SetValue(x, y, value);
		}
	}

	public T this[Point coordinates]
	{
		get
		{
			return GetValue(coordinates);
		}
		set
		{
			SetValue(coordinates, value);
		}
	}

	public T this[IHasCoordinates target]
	{
		get
		{
			return GetValue(target.Coordinates);
		}
		set
		{
			SetValue(target.Coordinates, value);
		}
	}

	public T this[int i]
	{
		get
		{
			return GetValueByIterator(i % Width, i / Height);
		}
		set
		{
			SetValueByIterator(i % Width, i / Height, value);
		}
	}

	public int Count => Size.X * Size.Y;

	public PointMatrix(IList<T> list)
	{
		Position = list.Min((T x) => x.Coordinates);
		Size = list.Max((T x) => x.Coordinates) - Position + Point.One;
		rows = new Row[Height];
		for (int i = 0; i < Height; i++)
		{
			rows[i] = new Row(Width);
		}
		foreach (T item in list)
		{
			SetValue(item.Coordinates, item);
		}
	}

	public PointMatrix(Point min, Point max)
	{
		Position = min;
		Size = max - min + Point.One;
		rows = new Row[Height];
		for (int i = 0; i < Height; i++)
		{
			rows[i] = new Row(Width);
		}
	}

	public PointMatrix(PointMatrix<T> other, Point min, Point max)
		: this(min, max)
	{
		for (int i = min.X; i <= max.X; i++)
		{
			for (int j = min.Y; j <= max.Y; j++)
			{
				this[i, j] = other[i, j];
			}
		}
	}

	public static PointMatrix<T> CreateBySize(Point size)
	{
		return CreateBySize(Point.Zero, size);
	}

	public static PointMatrix<T> CreateBySize(Point position, Point size)
	{
		return new PointMatrix<T>(position, size - Point.One);
	}

	public static PointMatrix<T> CreateByRegion(Point min, Point max)
	{
		return new PointMatrix<T>(min, max);
	}

	public static PointMatrix<T> CreateBySizeFromUnpointed(Point size, IList<T> unpoinedCollection, Action<T, Point> setPointFunc)
	{
		return CreateBySizeFromUnpointed(Point.Zero, size, unpoinedCollection, setPointFunc);
	}

	public static PointMatrix<T> CreateBySizeFromUnpointed(Point position, Point size, IList<T> unpoinedCollection, Action<T, Point> setPointFunc)
	{
		PointMatrix<T> pointMatrix = CreateBySize(position, size);
		if (pointMatrix.Count != unpoinedCollection.Count)
		{
			throw new IndexOutOfRangeException("Size not match");
		}
		for (int i = 0; i < unpoinedCollection.Count; i++)
		{
			Point outerById = pointMatrix.GetOuterById(i);
			setPointFunc(unpoinedCollection[i], outerById);
			pointMatrix[outerById] = unpoinedCollection[i];
		}
		return pointMatrix;
	}

	public T GetValueByIterator(int x, int y)
	{
		return rows[y].items[x];
	}

	public T GetValueByIterator(Point iterator)
	{
		return GetValueByIterator(iterator.X, iterator.Y);
	}

	public T GetValue(int x, int y)
	{
		return GetValueByIterator(x - X, y - Y);
	}

	public T GetValue(Point coordinates)
	{
		return GetValue(coordinates.X, coordinates.Y);
	}

	public bool TryGetValue(int x, int y, out T value)
	{
		value = default(T);
		if (!Contains(x, y))
		{
			return false;
		}
		value = GetValueByIterator(x - X, y - Y);
		return value != null;
	}

	public bool TryGetValue(Point point, out T value)
	{
		return TryGetValue(point.X, point.Y, out value);
	}

	public T SetValueByIterator(int x, int y, T value)
	{
		return rows[y].items[x] = value;
	}

	public T SetValueByIterator(Point iterator, T value)
	{
		return SetValueByIterator(iterator.X, iterator.Y, value);
	}

	public T SetValue(int x, int y, T value)
	{
		return SetValueByIterator(x - X, y - Y, value);
	}

	public T SetValue(Point coordinates, T value)
	{
		return SetValue(coordinates.X, coordinates.Y, value);
	}

	public bool Contains(Point point)
	{
		return Contains(point.X, point.Y);
	}

	public bool Contains(int x, int y)
	{
		if (x >= Min.X && y >= Min.Y && x <= Max.X)
		{
			return y <= Max.Y;
		}
		return false;
	}

	public Point GetInnerByOuter(Point outer)
	{
		return outer - Min;
	}

	public Point GetOuterByInner(Point inner)
	{
		return inner + Min;
	}

	public Point GetInnerById(int index)
	{
		return new Point(index % Width, index / Width);
	}

	public Point GetOuterById(int index)
	{
		return GetInnerById(index) + Min;
	}

	public void Swap(Point p1, Point p2)
	{
		if (!(p1 == p2))
		{
			T value = this[p2];
			this[p2] = this[p1];
			this[p1] = value;
		}
	}

	public List<T> ToList()
	{
		List<T> list = new List<T>();
		Row[] array = rows;
		foreach (Row row in array)
		{
			list.AddRange(row.items);
		}
		return list;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return new PointMatrixEnumerator(rows);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new PointMatrixEnumerator(rows);
	}
}
