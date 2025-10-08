using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenT.Types;

namespace GreenT.HornyScapes.Events.Content;

public class ContentSelectorGroup : Collection<IContentSelector>, IContentSelector, ISelector<ContentType>
{
	public ContentType Current { get; private set; }

	public bool IsMain()
	{
		return Current == ContentType.Main;
	}

	protected override void InsertItem(int index, IContentSelector item)
	{
		base.InsertItem(index, item);
		item.Select(Current);
	}

	protected override void SetItem(int index, IContentSelector item)
	{
		base.SetItem(index, item);
		item.Select(Current);
	}

	public void Initialize()
	{
		for (int num = base.Count - 1; num != -1; num--)
		{
			if (base[num] == null)
			{
				RemoveAt(num);
			}
		}
		Select(ContentType.Main);
	}

	public void Select(ContentType type)
	{
		if (Current == type)
		{
			return;
		}
		Current = type;
		using IEnumerator<IContentSelector> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.Select(type);
		}
	}
}
