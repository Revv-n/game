using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Meta.Decorations;
using Merge;
using StripClub.Model;
using StripClub.UI.Rewards;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GreenT.HornyScapes.ToolTips;

public class EventRewardDropViewToolTipOpener : DropViewToolTipOpener
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
	[Tooltip("Level")]
	private string levelKey;

	[SerializeField]
	private EventRewardCard sourceView;

	private LinkedContent storedContent;

	private GirlPromoOpener girlPromoOpener;

	[Inject]
	private void InnerInit(GirlPromoOpener girlPromoOpener)
	{
		this.girlPromoOpener = girlPromoOpener;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (!girlPromoOpener.TryToOpenGirlPromo(sourceView.Source.Content))
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
							if (linkedContent is BattlePassLevelLinkedContent)
							{
								base.Settings.KeyText = levelKey;
							}
						}
						else
						{
							base.Settings.KeyText = decorationKey;
						}
					}
					else
					{
						base.Settings.KeyText = localizationKey;
					}
				}
				else
				{
					base.Settings.KeyText = localizationKey + cardLinkedContent.Card.ID;
				}
			}
			else
			{
				base.Settings.KeyText = resourceKey + currencyLinkedContent.Currency.ToString().ToLower() + $".{currencyLinkedContent.CompositeIdentificator[0]}";
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
