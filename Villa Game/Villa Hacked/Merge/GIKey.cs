using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct GIKey : IEquatable<GIKey>, IComparable<GIKey>, IComparable
{
	[SerializeField]
	private int id;

	[SerializeField]
	private string collection;

	public int ID => id;

	public string Collection => collection;

	public GIKey(int id, string collection)
	{
		this.id = id;
		this.collection = collection;
	}

	public static implicit operator int(GIKey value)
	{
		return value.ID;
	}

	public static bool operator ==(GIKey a, GIKey b)
	{
		if (a.id == b.id)
		{
			return a.collection == b.collection;
		}
		return false;
	}

	public static bool operator !=(GIKey a, GIKey b)
	{
		if (a.id == b.id)
		{
			return a.collection != b.collection;
		}
		return true;
	}

	public static bool operator >(GIKey a, GIKey b)
	{
		return a.CompareTo(b) > 0;
	}

	public static bool operator >=(GIKey a, GIKey b)
	{
		return a.CompareTo(b) >= 0;
	}

	public static bool operator <(GIKey a, GIKey b)
	{
		return a.CompareTo(b) < 0;
	}

	public static bool operator <=(GIKey a, GIKey b)
	{
		return a.CompareTo(b) <= 0;
	}

	public override bool Equals(object obj)
	{
		if (obj is GIKey gIKey && id == gIKey.id)
		{
			return collection == gIKey.collection;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (-1056084179 * -1521134295 + id.GetHashCode()) * -1521134295 + collection.GetHashCode();
	}

	public int CompareTo(GIKey other)
	{
		if (collection.CompareTo(other.collection) != 0)
		{
			return collection.CompareTo(other.collection);
		}
		return id.CompareTo(other.id);
	}

	public int CompareTo(object obj)
	{
		return CompareTo((GIKey)obj);
	}

	public override string ToString()
	{
		return $"{Collection}{ID}";
	}

	public static GIKey Parse(string str)
	{
		string text = "";
		int num = str.Length - 1;
		if (!char.IsDigit(str[num]))
		{
			text = "1";
		}
		else
		{
			while (char.IsDigit(str[num]))
			{
				text = str[num] + text;
				num--;
			}
		}
		int num2 = int.Parse(text);
		string text2 = str.Substring(0, num + 1);
		return new GIKey(num2, text2);
	}

	public bool Equals(GIKey other)
	{
		if (ID == other.ID)
		{
			return Collection == other.Collection;
		}
		return false;
	}
}
