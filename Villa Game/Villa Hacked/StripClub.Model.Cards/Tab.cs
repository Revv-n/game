using UnityEngine;

namespace StripClub.Model.Cards;

public class Tab
{
	public int ID { get; }

	public int SortNumber { get; }

	public Sprite Icon { get; }

	public UnlockSettings UnlockSettings { get; }

	public Tab(int id, int sortNumber, Sprite icon, UnlockSettings unlockSettings)
	{
		ID = id;
		SortNumber = sortNumber;
		Icon = icon;
		UnlockSettings = unlockSettings;
	}

	public static int Comparsion(Tab x, Tab y)
	{
		if (x.ID > y.ID)
		{
			return 1;
		}
		if (x.ID == y.ID)
		{
			return 0;
		}
		return -1;
	}
}
