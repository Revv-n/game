using StripClub;
using UnityEngine;

namespace GreenT.HornyScapes;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Game Settings")]
public class GameSettings : ScriptableObject
{
	[SerializeField]
	private StripClub.GameSettings.RewTypeSettingsDictionary rewPlaceholder;

	[SerializeField]
	private StripClub.GameSettings.BonusSettingsDictionary bonusSettings;

	[SerializeField]
	private StripClub.GameSettings.LootBoxSpriteDictionary lootboxesSprites;

	[SerializeField]
	private StripClub.GameSettings.CurrencySettingsDictionary currencyPlaceholders;

	[SerializeField]
	private StripClub.GameSettings.CurrencySettingsDictionary currencySettings;

	public StripClub.GameSettings.CurrencySettingsDictionary CurrencyPlaceholder => currencyPlaceholders;

	public StripClub.GameSettings.RewTypeSettingsDictionary RewPlaceholder => rewPlaceholder;

	public StripClub.GameSettings.BonusSettingsDictionary BonusSettings => bonusSettings;

	public StripClub.GameSettings.CurrencySettingsDictionary CurrencySettings => currencySettings;

	public StripClub.GameSettings.LootBoxSpriteDictionary LootBoxSprites => lootboxesSprites;
}
