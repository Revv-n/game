using System.Linq;

public sealed class FilterNode<T>
{
	public FilterType type;

	public T[] list;

	public bool IsWhite
	{
		get
		{
			return type == FilterType.White;
		}
		set
		{
			type = FilterType.White;
		}
	}

	public bool IsBlack
	{
		get
		{
			return type == FilterType.Black;
		}
		set
		{
			type = FilterType.Black;
		}
	}

	public static FilterNode<T> Empty => new FilterNode<T>(FilterType.Black);

	public bool IsBlocks(T value)
	{
		return list.Any((T x) => x.Equals(value)) == IsBlack;
	}

	public bool IsAllows(T value)
	{
		return !IsBlocks(value);
	}

	public FilterNode(FilterType type, params T[] list)
	{
		this.type = type;
		this.list = list;
	}
}
