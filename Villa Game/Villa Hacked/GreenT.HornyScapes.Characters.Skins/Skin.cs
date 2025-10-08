using System;
using GreenT.Data;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Characters.Skins;

[MementoHolder]
public class Skin : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public bool IsOwned { get; private set; }

		public Memento(Skin skin)
			: base(skin)
		{
			IsOwned = skin.IsOwned;
		}
	}

	private const string nameKeyTemplate = "content.character.skins.{0}.name";

	private ISkinData data;

	private readonly string uniqueKey;

	public int ID { get; }

	public int GirlID { get; }

	public int OrderNumber { get; }

	public Rarity Rarity { get; }

	public ILocker Locker { get; }

	public string NameKey => $"content.character.skins.{ID}.name";

	public string UnlockMessageKey { get; }

	public ISkinData Data
	{
		get
		{
			if (data == null)
			{
				if (Locker.IsOpen.Value)
				{
					new ArgumentNullException("data", $"({ID}) Data weren't loaded yet.").LogException();
				}
				else
				{
					new ArgumentNullException("data", $"({ID}) Data weren't loaded yet. Because locker is still closed. Locker: " + Locker.ToString()).LogException();
				}
			}
			return data;
		}
		private set
		{
			data = value;
		}
	}

	public bool IsDataEmpty => data == null;

	public bool IsOwned { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void Own()
	{
		IsOwned = true;
	}

	public Skin(SkinMapper mapper, ILocker locker)
	{
		ID = mapper.id;
		GirlID = mapper.girl_id;
		OrderNumber = mapper.order_number;
		Rarity = mapper.rarity;
		UnlockMessageKey = mapper.unlock_message;
		Locker = locker;
		uniqueKey = "character.skin." + ID;
	}

	public override string ToString()
	{
		return base.ToString() + " ID:" + ID + " for сharacter ID:" + GirlID;
	}

	public void Insert(ISkinData data)
	{
		if (!IsDataEmpty)
		{
			Debug.LogWarning(this?.ToString() + ": SkinData is already set");
		}
		Data = data;
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		IsOwned = memento2.IsOwned;
	}
}
