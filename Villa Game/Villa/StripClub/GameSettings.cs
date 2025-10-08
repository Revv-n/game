using System;
using GreenT.Bonus;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using UnityEngine;

namespace StripClub;

[CreateAssetMenu(menuName = "StripClub/Settings/Game settings")]
public class GameSettings : ScriptableObject
{
	[Serializable]
	public class CurrencySettingsDictionary : SerializableDictionary<SimpleCurrency.CurrencyKey, CurrencySettings>
	{
		private SimpleCurrency.CurrencyKey _commonCurrencyKey;

		private CurrenciesService _currenciesService;

		public CurrencySettings this[CurrencyType currencyType, CompositeIdentificator identificator = default(CompositeIdentificator)] => AlternativeGet(currencyType, identificator);

		public void SetupCurrenciesService(CurrenciesService currenciesService)
		{
			_currenciesService = currenciesService;
		}

		public bool TryGetValue(CurrencyType currencyType, out CurrencySettings currencySettings, CompositeIdentificator identificator = default(CompositeIdentificator))
		{
			_currenciesService.CheckoutCurrencySettings(currencyType, identificator);
			currencySettings = null;
			SimpleCurrency.CurrencyKey currencyKey = GetCurrencyKey(currencyType, identificator);
			if (ContainsKey(currencyKey))
			{
				currencySettings = base[currencyKey];
				return true;
			}
			return false;
		}

		public bool TryAdd(CurrencyType currencyType, CurrencySettings currencySettings, CompositeIdentificator identificator = default(CompositeIdentificator))
		{
			SimpleCurrency.CurrencyKey currencyKey = GetCurrencyKey(currencyType, identificator);
			if (ContainsKey(currencyKey))
			{
				return false;
			}
			Add(currencyKey, currencySettings);
			return true;
		}

		public bool Contains(CurrencyType type, CompositeIdentificator identificator)
		{
			SetupCommonCurrencyKey(type, identificator);
			return ContainsKey(_commonCurrencyKey);
		}

		public CurrencySettings Get(CurrencyType currencyType, CompositeIdentificator identificator)
		{
			return base[GetCurrencyKey(currencyType, identificator)];
		}

		private CurrencySettings AlternativeGet(CurrencyType currencyType, CompositeIdentificator identificator)
		{
			_currenciesService.CheckoutCurrencySettings(currencyType, identificator);
			return base[GetCurrencyKey(currencyType, identificator)];
		}

		private SimpleCurrency.CurrencyKey GetCurrencyKey(CurrencyType currencyType, CompositeIdentificator identificator = default(CompositeIdentificator))
		{
			SetupCommonCurrencyKey(currencyType, identificator);
			return _commonCurrencyKey;
		}

		private void SetupCommonCurrencyKey(CurrencyType currencyType, CompositeIdentificator identificator)
		{
			_commonCurrencyKey.CurrencyType = currencyType;
			if (identificator.Identificators == null)
			{
				identificator = new CompositeIdentificator(default(int));
			}
			_commonCurrencyKey.Identificator = identificator;
		}
	}

	[Serializable]
	public class EditorCurrencySettingsDictionary : SerializableDictionary<CurrencyType, CurrencySettings>
	{
	}

	[Serializable]
	public class RewTypeSettingsDictionary : SerializableDictionary<RewType, RewSettings>
	{
	}

	[Serializable]
	public class BonusSettingsDictionary : SerializableDictionary<BonusType, BonusSettings>
	{
	}

	[Serializable]
	public class LanguageFlagDictionary : SerializableDictionary<Language, Sprite>
	{
	}

	[Serializable]
	public class AudioEffectsDictionary : SerializableDictionary<AudioEffectType, AudioClip>
	{
	}

	[Serializable]
	public class IntToIntDictionary : SerializableDictionary<int, int>
	{
	}

	[Serializable]
	public class LootBoxSpriteDictionary : SerializableDictionary<Rarity, Sprite>
	{
	}

	[SerializeField]
	private CurrencySettingsDictionary currencySettings;

	[SerializeField]
	private AudioEffectsDictionary audioEffects;

	[SerializeField]
	private AudioClip soundTrack;

	public CurrencySettingsDictionary CurrencySettings => currencySettings;

	public AudioEffectsDictionary AudioEffects => audioEffects;

	public AudioClip SoundTrack => soundTrack;
}
