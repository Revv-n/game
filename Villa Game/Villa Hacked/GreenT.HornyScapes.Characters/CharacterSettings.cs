using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Characters;

[MementoHolder]
public class CharacterSettings : ISavableState, IDisposable
{
	[Serializable]
	public class ThisMemento : Memento
	{
		public int ID { get; private set; }

		public int AvatarNumber { get; private set; }

		public int Level { get; private set; }

		public int Progress { get; private set; }

		public bool IsNew { get; private set; }

		public int SkinID { get; private set; }

		public List<int> OwnedSkins { get; private set; }

		public ThisMemento(CharacterSettings character)
			: base(character)
		{
			ID = character.Public.ID;
			AvatarNumber = character.AvatarNumber;
			Level = character.Promote.Level.Value;
			Progress = character.Promote.Progress.Value;
			IsNew = character.Promote.IsNew.Value;
			SkinID = character.SkinID;
			OwnedSkins = character.OwnedSkins;
		}
	}

	private readonly string uniqueKey;

	private Subject<CharacterSettings> onUpdate;

	protected CompositeDisposable promoteUpdateStream = new CompositeDisposable();

	public IObservable<CharacterSettings> OnUpdate { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public ICharacter Public { get; private set; }

	public int AvatarNumber { get; private set; }

	public int SkinID { get; private set; }

	public List<int> OwnedSkins { get; private set; }

	public IPromote Promote { get; protected set; }

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public CharacterSettings(ICharacter characterInfo, IPromote promote)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		AvatarNumber = 0;
		SkinID = 0;
		OwnedSkins = new List<int> { 0 };
		Public = characterInfo;
		Promote = promote;
		onUpdate = new Subject<CharacterSettings>();
		OnUpdate = Observable.AsObservable<CharacterSettings>((IObservable<CharacterSettings>)onUpdate);
		uniqueKey = "character_" + Public.ID;
	}

	public void Init()
	{
		promoteUpdateStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Skip<int>((IObservable<int>)Promote.Level, 1), (Action<int>)TrySetNextAvatar), (ICollection<IDisposable>)promoteUpdateStream);
	}

	public void SetAvatar(int number)
	{
		if (Public.GetBundleData().Avatars.ElementAt(number).Key <= Promote.Level.Value)
		{
			AvatarNumber = number;
			onUpdate.OnNext(this);
		}
	}

	public void SetSkin(int skinID)
	{
		if (skinID != SkinID)
		{
			SkinID = skinID;
			onUpdate.OnNext(this);
		}
	}

	protected void TrySetNextAvatar(int level)
	{
		int num = 0;
		foreach (int key in Public.GetBundleData().Avatars.Keys)
		{
			if (key == level)
			{
				SetAvatar(num);
				break;
			}
			num++;
		}
	}

	public Memento SaveState()
	{
		return new ThisMemento(this);
	}

	public void LoadState(Memento memento)
	{
		if (memento is CharacterProgressMemento characterProgressMemento)
		{
			CharacterProgressMemento characterProgressMemento2 = characterProgressMemento;
			Promote.Init(characterProgressMemento2.Level, characterProgressMemento2.Progress);
			Promote.IsNew.Value = characterProgressMemento2.IsNew;
			AvatarNumber = characterProgressMemento2.AvatarNumber;
		}
		else if (memento is ThisMemento thisMemento)
		{
			ThisMemento thisMemento2 = thisMemento;
			Promote.Init(thisMemento2.Level, thisMemento2.Progress);
			Promote.IsNew.Value = thisMemento2.IsNew;
			AvatarNumber = thisMemento2.AvatarNumber;
			SkinID = thisMemento2.SkinID;
			OwnedSkins = thisMemento2.OwnedSkins ?? new List<int>();
			if (!OwnedSkins.Contains(0))
			{
				OwnedSkins.Add(0);
			}
		}
		onUpdate.OnNext(this);
	}

	public void Dispose()
	{
		onUpdate.Dispose();
		promoteUpdateStream.Dispose();
	}
}
