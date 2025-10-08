using System;
using UnityEngine;

[Serializable]
public struct RayClickOrder : IComparable
{
	[SerializeField]
	private int order;

	[SerializeField]
	private int layer;

	public int Order
	{
		get
		{
			return order;
		}
		set
		{
			order = value;
		}
	}

	public int Layer
	{
		get
		{
			return layer;
		}
		set
		{
			layer = value;
		}
	}

	public RayClickOrder(int order, int layer)
	{
		this.order = order;
		this.layer = layer;
	}

	public override bool Equals(object obj)
	{
		RayClickOrder rayClickOrder = (RayClickOrder)obj;
		if (order == rayClickOrder.order)
		{
			return layer == rayClickOrder.layer;
		}
		return false;
	}

	public int CompareTo(object obj)
	{
		RayClickOrder rayClickOrder = (RayClickOrder)obj;
		if (layer != rayClickOrder.layer)
		{
			return layer.CompareTo(rayClickOrder.layer);
		}
		return order.CompareTo(rayClickOrder.order);
	}

	public override int GetHashCode()
	{
		return (-262534107 * -1521134295 + order.GetHashCode()) * -1521134295 + layer.GetHashCode();
	}

	public static bool operator ==(RayClickOrder a, RayClickOrder b)
	{
		if (a.order == b.order)
		{
			return a.layer == b.layer;
		}
		return false;
	}

	public static bool operator !=(RayClickOrder a, RayClickOrder b)
	{
		if (a.order == b.order)
		{
			return a.layer != b.layer;
		}
		return true;
	}

	public static bool operator >(RayClickOrder a, RayClickOrder b)
	{
		if (a.layer != b.layer)
		{
			return a.layer > b.layer;
		}
		return a.order > b.order;
	}

	public static bool operator <(RayClickOrder a, RayClickOrder b)
	{
		if (a.layer != b.layer)
		{
			return a.layer < b.layer;
		}
		return a.order < b.order;
	}

	public static bool operator >=(RayClickOrder a, RayClickOrder b)
	{
		if (a.layer != b.layer)
		{
			return a.layer >= b.layer;
		}
		return a.order >= b.order;
	}

	public static bool operator <=(RayClickOrder a, RayClickOrder b)
	{
		if (a.layer != b.layer)
		{
			return a.layer <= b.layer;
		}
		return a.order <= b.order;
	}
}
