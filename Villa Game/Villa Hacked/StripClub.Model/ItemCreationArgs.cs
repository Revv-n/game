using System;

namespace StripClub.Model;

public class ItemCreationArgs
{
	public IItemInfo Info { get; set; }

	public int Amount { get; set; }

	public DateTime ActiveDueDate { get; set; }

	public int ActiveCount { get; set; }

	public uint Level { get; set; }

	public int SkinID { get; set; }

	public IPlayerBasics PlayerBasics { get; set; }

	public ItemCreationArgs(IItemInfo info)
	{
		Info = info;
	}

	public ItemCreationArgs SetAmount(int amount)
	{
		Amount = amount;
		return this;
	}

	public ItemCreationArgs SetActivityParameters(DateTime dueDate, int activeCount)
	{
		ActiveDueDate = dueDate;
		ActiveCount = activeCount;
		return this;
	}

	public ItemCreationArgs SetLevel(uint level)
	{
		Level = level;
		return this;
	}

	public ItemCreationArgs SetSkinID(int skinId)
	{
		SkinID = skinId;
		return this;
	}

	public ItemCreationArgs SetPlayerBasics(IPlayerBasics playerBasics)
	{
		PlayerBasics = playerBasics;
		return this;
	}
}
