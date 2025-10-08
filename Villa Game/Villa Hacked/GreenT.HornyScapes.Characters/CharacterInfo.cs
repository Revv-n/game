using System;
using System.Collections.Generic;
using GreenT.Bonus;
using GreenT.HornyScapes.Dates.Models;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Character;
using UnityEngine;

namespace GreenT.HornyScapes.Characters;

public class CharacterInfo : ICharacter, ICard
{
	private readonly string name;

	private CharacterData bundleData;

	public int ID { get; }

	public int OrderNumber { get; }

	public int[] NudeLevels { get; }

	public ContentType ContentType { get; }

	public LoadType LoadType { get; }

	public Rarity Rarity { get; }

	public ILocker DisplayCondition { get; }

	public ILocker PreloadLocker { get; }

	public IBonus Bonus { get; }

	public int PromotePatternsID { get; }

	public int GroupID => 1;

	public string NameKey { get; }

	public string DescriptionKey { get; }

	public bool IsBundleDataReady => bundleData != null;

	private CharacterData BundleData => GetBundleData();

	public IDictionary<int, Sprite> Avatars => BundleData?.Avatars;

	public Sprite DefaultAvatar => BundleData?.DefaultAvatar;

	public Sprite MessengerAvatar => BundleData?.MessengerAvatar;

	public Sprite MessengerTaskAvatar => BundleData?.MessengerTaskAvatar;

	public CharacterStories CharacterStories { get; private set; }

	public DateCharacterStories DateStories { get; private set; }

	public int RelationsipId { get; }

	public Sprite RewardsNewGirlSprite => BundleData?.SplashArt;

	public BankImages BankImages => BundleData?.BankImages;

	public Sprite ProgressBarIcon => BundleData?.ProgressBarIcon;

	public CharacterInfo(int id, int orderNumber, int[] nudeLevels, int promotePatternsID, string name, string nameKey, string descriptionKey, Rarity rarity, ContentType contentType, LoadType loadType, IBonus bonus, ILocker displayConditionLocker, ILocker preloadLocker, int relationsipID)
	{
		ID = id;
		NameKey = nameKey;
		DescriptionKey = descriptionKey;
		OrderNumber = orderNumber;
		NudeLevels = nudeLevels;
		ContentType = contentType;
		LoadType = loadType;
		Rarity = rarity;
		Bonus = bonus;
		PromotePatternsID = promotePatternsID;
		DisplayCondition = displayConditionLocker;
		PreloadLocker = preloadLocker;
		this.name = name;
		RelationsipId = relationsipID;
	}

	public CharacterData GetBundleData()
	{
		if (bundleData == null)
		{
			BundleDataNullReferenceException().LogException();
		}
		return bundleData;
	}

	public CharacterStories GetStory()
	{
		return CharacterStories;
	}

	public DateCharacterStories GetDateStory()
	{
		return DateStories;
	}

	public void Set(CharacterData characterData)
	{
		bundleData = characterData;
		bundleData.SetupAvatars(NudeLevels);
	}

	public void SetStory(CharacterStories characterStories)
	{
		CharacterStories = characterStories;
	}

	public void SetDateStory(DateCharacterStories dateStories)
	{
		DateStories = dateStories;
	}

	private NullReferenceException BundleDataNullReferenceException()
	{
		return new NullReferenceException("[CharacterInfo_GetBundleData] " + ToString() + " bundle data hasn't been loaded yet");
	}

	public override string ToString()
	{
		return $"{name}{{{ID}:{GroupID}}} card ";
	}
}
