using System;
using System.Collections.Generic;
using GreenT;
using UnityEngine;

namespace StripClub.Model.Character;

[CreateAssetMenu(fileName = "CharacterView", menuName = "StripClub/Character/Settings")]
public class CharacterData : ScriptableObject
{
	public static string bundleNamePrefix = "employee/";

	[SerializeField]
	private int characterID = -1;

	[SerializeField]
	private Sprite defautAvatar;

	[SerializeField]
	private Sprite messengerAvtar;

	[SerializeField]
	private Sprite messengerTaskAvatar;

	[SerializeField]
	private Sprite squareIcon;

	[SerializeField]
	private List<Sprite> cardImages;

	[SerializeField]
	private Sprite splashArt;

	[SerializeField]
	private BankImages bankImages;

	[SerializeField]
	private Sprite progressBarIcon;

	[SerializeField]
	private Sprite spriteForBonus;

	public int ID
	{
		get
		{
			return characterID;
		}
		set
		{
			characterID = value;
		}
	}

	public Sprite DefaultAvatar => defautAvatar;

	public Sprite MessengerAvatar => messengerAvtar;

	public Sprite MessengerTaskAvatar => messengerTaskAvatar;

	public List<Sprite> CardImages => cardImages;

	public Sprite SplashArt => splashArt;

	public BankImages BankImages => bankImages;

	public Sprite ProgressBarIcon => progressBarIcon;

	public Sprite SpriteForBonus => spriteForBonus;

	public Sprite SquareIcon => squareIcon;

	public IDictionary<int, Sprite> Avatars { get; private set; }

	public void SetupAvatars(int[] levels)
	{
		Avatars = new Dictionary<int, Sprite>();
		try
		{
			for (int i = 0; i != levels.Length; i++)
			{
				Avatars.Add(levels[i], CardImages[i]);
			}
		}
		catch (Exception innerException)
		{
			string errMsg = "Character with id " + ID + " have different number of nude_level's (" + levels.Length + ") in table and number of CardImages in CharacterData prefab (" + CardImages.Count + ")";
			throw innerException.SendException(errMsg);
		}
	}

	public Sprite GetLevelAvatar(int level)
	{
		if (!Avatars.ContainsKey(level))
		{
			throw new NullReferenceException(string.Format("[{0}_[{1}] Character_ID_{2} does not contain avatars for level {3}", "CharacterData", "GetLevelAvatar", ID, level));
		}
		return Avatars[level];
	}
}
