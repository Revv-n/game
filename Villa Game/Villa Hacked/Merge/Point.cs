using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct Point : IEquatable<Point>, IComparable<Point>
{
	public static readonly Point Zero = new Point(0, 0);

	public static readonly Point One = new Point(1, 1);

	public static readonly Point Up = new Point(0, 1);

	public static readonly Point Down = new Point(0, -1);

	public static readonly Point Right = new Point(1, 0);

	public static readonly Point Left = new Point(-1, 0);

	[SerializeField]
	private int x;

	[SerializeField]
	private int y;

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Point Add(int x, int y)
	{
		return new Point(this.x + x, this.y + y);
	}

	public Point AddSize(int x, int y)
	{
		return new Point(this.x + x - 1, this.y + y - 1);
	}

	public Point AddSize(Point size)
	{
		return AddSize(size.x, size.y);
	}

	public Point AddX(int x)
	{
		return new Point(this.x + x, y);
	}

	public Point AddY(int y)
	{
		return new Point(x, this.y + y);
	}

	public Point DistanceTo(Point target)
	{
		return target - this;
	}

	public int Distance(Point target, bool useDiagonal = true)
	{
		Point point = DistanceTo(target);
		if (useDiagonal)
		{
			return Mathf.Max(Mathf.Abs(point.x), Mathf.Abs(point.y));
		}
		return Mathf.Abs(point.x) + Mathf.Abs(point.y);
	}

	public bool Is(int x, int y)
	{
		if (this.x == x)
		{
			return this.y == y;
		}
		return false;
	}

	public static bool operator ==(Point a, Point b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Point a, Point b)
	{
		return !a.Equals(b);
	}

	public static bool operator <(Point a, Point b)
	{
		return a.CompareTo(b) < 0;
	}

	public static bool operator <=(Point a, Point b)
	{
		return a.CompareTo(b) <= 0;
	}

	public static bool operator >(Point a, Point b)
	{
		return a.CompareTo(b) > 0;
	}

	public static bool operator >=(Point a, Point b)
	{
		return a.CompareTo(b) >= 0;
	}

	public static Point operator +(Point a, Point b)
	{
		return new Point(a.x + b.x, a.y + b.y);
	}

	public static Point operator -(Point a, Point b)
	{
		return new Point(a.x - b.x, a.y - b.y);
	}

	public static Point operator *(Point a, Point b)
	{
		return new Point(a.x * b.x, a.y * b.y);
	}

	public static Point operator /(Point a, Point b)
	{
		return new Point(a.x / b.x, a.y / b.y);
	}

	public static Point operator *(Point a, int mul)
	{
		return new Point(a.x * mul, a.y * mul);
	}

	public static Point operator /(Point a, int mul)
	{
		return new Point(a.x / mul, a.y / mul);
	}

	public static implicit operator Point(Vector2Int value)
	{
		return new Point(value.x, value.y);
	}

	public static implicit operator Vector2Int(Point value)
	{
		return new Vector2Int(value.x, value.y);
	}

	public static implicit operator Vector2(Point value)
	{
		return new Vector2(value.x, value.y);
	}

	public static implicit operator Vector3(Point value)
	{
		return new Vector3(value.x, value.y, 0f);
	}

	public bool Equals(Point other)
	{
		if (x == other.x)
		{
			return y == other.y;
		}
		return false;
	}

	public int CompareTo(Point other)
	{
		if (x != other.x)
		{
			return x.CompareTo(other.x);
		}
		return y.CompareTo(other.y);
	}

	public override int GetHashCode()
	{
		return (1502939027 * -1521134295 + x.GetHashCode()) * -1521134295 + y.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Point other))
		{
			return false;
		}
		return Equals(other);
	}

	public override string ToString()
	{
		return $"[{x}:{y}]";
	}
}
