using System;
using GreenT.Data;
using ModestTree;
using StripClub.Extensions;
using StripClub.Model.Data;

namespace StripClub.Model;

public class Item : ISavableState, IEquatable<Item>
{
	public IItemInfo Info { get; protected set; }

	public BoundedValue<int> Amount { get; set; }

	public string GUID => Info.Guid.ToString();

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Item(IItemInfo item, int amount)
	{
		Assert.IsNotNull(item);
		Info = item;
		Amount = new BoundedValue<int>(amount, int.MaxValue, 0);
	}

	public string UniqueKey()
	{
		return Info.Guid.ToString();
	}

	public virtual Memento SaveState()
	{
		return new ItemMapper(this);
	}

	public virtual void LoadState(Memento memento)
	{
		ItemMapper itemMapper = (ItemMapper)memento;
		if (itemMapper.UniqueKey.Equals(UniqueKey()))
		{
			throw new ArgumentException("Wrong key");
		}
		Amount.Value = itemMapper.Amount;
	}

	public override int GetHashCode()
	{
		return Info.GetHashCode();
	}

	public bool Equals(Item other)
	{
		if (other != null)
		{
			return Info.Guid.Equals(other.Info.Guid);
		}
		return false;
	}
}
