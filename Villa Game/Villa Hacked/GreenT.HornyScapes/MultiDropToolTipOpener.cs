using System;
using GreenT.Bonus;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.ToolTips;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes;

public class MultiDropToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	protected MultiDropType _dropType;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Card)]
	protected int _girlId;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Currency)]
	protected CurrencyType _currencyType;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Currency)]
	protected int _currencyId;

	[SerializeField]
	[Tooltip("Spawner")]
	[ShowIfEnum("_dropType", MultiDropType.MergeItem)]
	protected string _additionalKey;

	[SerializeField]
	[Tooltip("Spawner Collection")]
	[ShowIfEnum("_dropType", MultiDropType.MergeItem)]
	protected string _spawnerCollectionKey;

	[SerializeField]
	[Tooltip("Is Spawner")]
	[ShowIfEnum("_dropType", MultiDropType.MergeItem)]
	protected bool _isSpawner;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Decoration)]
	protected int _decorationId;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Lootbox)]
	protected Rarity _lootboxRarity;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Skin)]
	protected int _skinId;

	[SerializeField]
	[ShowIfEnum("_dropType", MultiDropType.Booster)]
	protected BonusType _bonusType;

	private GirlPromoOpener _girlPromoOpener;

	private CharacterManager _characterManager;

	[Inject]
	private void Init(GirlPromoOpener girlPromoOpener, CharacterManager characterManager)
	{
		_girlPromoOpener = girlPromoOpener;
		_characterManager = characterManager;
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (localizationKey == string.Empty)
		{
			UpdateLocalizationKey();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (_dropType == MultiDropType.Card)
		{
			_girlPromoOpener.TryToOpenGirlPromo(_characterManager.Get(_girlId));
		}
		else
		{
			base.OnPointerClick(eventData);
		}
	}

	protected void UpdateLocalizationKey()
	{
		switch (_dropType)
		{
		case MultiDropType.Card:
			localizationKey = "ui.hint.girl.";
			break;
		case MultiDropType.Currency:
			localizationKey = "ui.hint.resource.";
			break;
		case MultiDropType.MergeItem:
			localizationKey = "ui.hint.item";
			_additionalKey = "ui.hint.spawner.";
			break;
		case MultiDropType.Decoration:
			localizationKey = "ui.hint.decoration.";
			break;
		case MultiDropType.Lootbox:
			localizationKey = "ui.hint.lootbox.";
			break;
		case MultiDropType.Skin:
			localizationKey = "content.character.skins.{0}.name";
			break;
		case MultiDropType.Booster:
			localizationKey = "ui.hint.bonus.energy_cap";
			break;
		}
	}

	protected override void SetSettings()
	{
		switch (_dropType)
		{
		case MultiDropType.Card:
			base.Settings.KeyText = localizationKey + _girlId;
			break;
		case MultiDropType.Currency:
			base.Settings.KeyText = localizationKey + _currencyType.ToString().ToLower() + $".{_currencyId}";
			break;
		case MultiDropType.MergeItem:
			base.Settings.KeyText = (_isSpawner ? (_additionalKey + _spawnerCollectionKey.ToLower()) : localizationKey);
			break;
		case MultiDropType.Decoration:
			base.Settings.KeyText = localizationKey + _decorationId;
			break;
		case MultiDropType.Lootbox:
			base.Settings.KeyText = localizationKey + _lootboxRarity.ToString().ToLower();
			break;
		case MultiDropType.Skin:
			localizationKey = string.Format(localizationKey, _skinId);
			base.Settings.KeyText = localizationKey;
			break;
		case MultiDropType.Booster:
			UpdateBoosterKeyText();
			base.Settings.KeyText = localizationKey;
			break;
		}
	}

	private void UpdateBoosterKeyText()
	{
		switch (_bonusType)
		{
		case BonusType.increaseDropValue:
			localizationKey += ".increase_drop_value";
			break;
		case BonusType.increaseProductionValue:
			localizationKey += ".increase_production_value";
			break;
		case BonusType.decreaseReloadTime:
			localizationKey += ".decrease_reload_time";
			break;
		case BonusType.IncreaseBaseEnergy:
			localizationKey += ".increase_base_energy";
			break;
		case BonusType.IncreaseEnergyRechargeSpeed:
			localizationKey += ".increase_energy_recharge_speed";
			break;
		case BonusType.FreeSummon:
			localizationKey += ".free_summon";
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
