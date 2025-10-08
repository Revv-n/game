using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events.BattlePassRewardCards;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Meta.Decorations;
using GreenT.Localizations;
using Merge;
using StripClub.Model;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class BattlePassRewardDropViewToolTipOpener : DropViewToolTipOpener
{
	[SerializeField]
	[Tooltip("Spawner")]
	private string additionalKey;

	[SerializeField]
	[Tooltip("Resource")]
	private string resourceKey;

	[SerializeField]
	[Tooltip("Decoration")]
	private string decorationKey;

	[SerializeField]
	[Tooltip("Skin")]
	private string skinKey;

	[SerializeField]
	private BattlePassRewardCard sourceView;

	private string lootboxHintLocalizationKey = "ui.hint.";

	private LinkedContent storedContent;

	private LootBoxSettings _lootBoxSettings;

	private GirlPromoOpener _girlPromoOpener;

	private LocalizationService _localizationService;

	[Inject]
	private void Construct(LootBoxSettings lootBoxSettings, GirlPromoOpener girlPromoOpener, LocalizationService localizationService)
	{
		_lootBoxSettings = lootBoxSettings;
		_girlPromoOpener = girlPromoOpener;
		_localizationService = localizationService;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!_girlPromoOpener.TryToOpenGirlPromo(sourceView.Source.Content))
		{
			base.OnPointerClick(eventData);
		}
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (string.IsNullOrEmpty(localizationKey))
		{
			localizationKey = "ui.hint.item";
		}
		if (string.IsNullOrEmpty(additionalKey))
		{
			additionalKey = "ui.hint.spawner.";
		}
		if (string.IsNullOrEmpty(resourceKey))
		{
			resourceKey = "ui.hint.resource.";
		}
		if (string.IsNullOrEmpty(decorationKey))
		{
			decorationKey = "ui.hint.decoration";
		}
		if (string.IsNullOrEmpty(skinKey))
		{
			decorationKey = "content.character.skins.hint";
		}
	}

	protected override void SetSettings()
	{
		if (storedContent != sourceView.Source.Content)
		{
			SetContentLocalizationKey(sourceView.Source.Content);
		}
	}

	private void SetContentLocalizationKey(LinkedContent linkedContent)
	{
		if (!(linkedContent is MergeItemLinkedContent mergeItemLinkedContent))
		{
			if (!(linkedContent is CurrencyLinkedContent currencyLinkedContent))
			{
				if (!(linkedContent is CardLinkedContent cardLinkedContent))
				{
					if (!(linkedContent is SkinLinkedContent))
					{
						if (!(linkedContent is DecorationLinkedContent))
						{
							if (linkedContent is LootboxLinkedContent lootboxLinkedContent)
							{
								string key = lootboxHintLocalizationKey + _lootBoxSettings.GetName(lootboxLinkedContent.GetRarity()).ToLower() + "box";
								base.Settings.KeyText = _localizationService.Text(key);
							}
						}
						else
						{
							base.Settings.KeyText = decorationKey;
						}
					}
					else
					{
						base.Settings.KeyText = skinKey;
					}
				}
				else
				{
					base.Settings.KeyText = localizationKey + cardLinkedContent.Card.ID;
				}
			}
			else
			{
				base.Settings.KeyText = resourceKey + currencyLinkedContent.Currency.ToString().ToLower();
			}
		}
		else
		{
			bool flag = mergeItemLinkedContent.GameItemConfig.HasModule(GIModuleType.ClickSpawn);
			base.Settings.KeyText = (flag ? (additionalKey + mergeItemLinkedContent.GameItemConfig.Key.Collection.ToLower()) : localizationKey);
		}
		storedContent = linkedContent;
	}
}
