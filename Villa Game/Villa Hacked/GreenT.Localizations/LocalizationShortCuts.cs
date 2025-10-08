using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GreenT.HornyScapes.Characters;
using StripClub.Model.Cards;

namespace GreenT.Localizations;

public class LocalizationShortCuts
{
	private const string contentPrefix = "content.";

	private const string prefSufix = "pref.";

	private readonly Dictionary<Type, string> objectTypeTable = new Dictionary<Type, string>
	{
		{
			typeof(CharacterInfo),
			"character."
		},
		{
			typeof(ICharacter),
			"character."
		}
	};

	private LocalizationProvider localization;

	private const string LOOTBOX_SUFIX = "firstshow.";

	public LocalizationShortCuts(LocalizationProvider localization)
	{
		this.localization = localization;
	}

	private StringBuilder PrefKey(Type type, int objectID = -1)
	{
		try
		{
			KeyValuePair<Type, string> keyValuePair = objectTypeTable.First((KeyValuePair<Type, string> _pair) => _pair.Key.IsAssignableFrom(type));
			StringBuilder stringBuilder = new StringBuilder("content.");
			stringBuilder.Append(keyValuePair.Value);
			if (objectID >= 0)
			{
				stringBuilder.Append(objectID).Append('.');
			}
			return stringBuilder;
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}

	public string ObjectPrefKey(string localizationKey, int paramNumber)
	{
		StringBuilder stringBuilder = new StringBuilder(localizationKey);
		return paramNumber switch
		{
			0 => stringBuilder.Append(".name").ToString(), 
			1 => stringBuilder.Append(".description").ToString(), 
			_ => stringBuilder.Append(".pref.").Append(paramNumber).ToString(), 
		};
	}

	public string LootboxPrefValue(ICard card, int paramNumber)
	{
		return PrefKey(card.GetType()).Append("firstshow.").Append($"{card.ID}.").Append("pref.")
			.Append(paramNumber)
			.ToString();
	}

	public string LootboxPrefName(ICard card, int paramNumber)
	{
		return PrefKey(card.GetType()).Append("firstshow.").Append("pref.").Append(paramNumber)
			.ToString();
	}

	public string CardParameterValueKey(ICard card, int paramNumber)
	{
		return PrefKey(card.GetType(), card.ID).Append("pref.").Append(paramNumber).ToString();
	}

	public string CardParameterNameKey(ICard card, int prefNumber)
	{
		return PrefKey(card.GetType()).Append("pref.").Append(prefNumber).ToString();
	}

	public IEnumerable<int> GetPrefsNumbers(ICard card)
	{
		int iD = card.ID;
		Type type = card.GetType();
		string text = PrefKey(type, iD).Append("pref.").ToString();
		Regex regex = new Regex("^" + text);
		return from _key in localization.LocalizationKeys
			where regex.IsMatch(_key)
			select int.Parse(regex.Replace(_key, ""));
	}
}
