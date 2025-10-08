using GreenT.AssetBundles;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Characters.Skins.Content;
using GreenT.HornyScapes.Content;
using GreenT.HornyScapes.Meta.Decorations;
using Merge;
using StripClub.Model;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public class PromoCardView : MultiDropToolTipOpener, IView<LinkedContent>, IView
{
	[SerializeField]
	private StatableComponent _background;

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _amount;

	private const int DEFAULT_ITEM_BACKGRROUND = 10;

	private const int COUNTABLE_ITEM_BACKGRROUND = 11;

	[Inject]
	private FakeAssetService _fakeAssetService;

	public int SiblingIndex { get; set; }

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void Display(bool display)
	{
		if (IsActive() != display)
		{
			base.gameObject.SetActive(display);
		}
	}

	public void Set(LinkedContent reward)
	{
		if (!(reward is CardLinkedContent) && !(reward is SkinLinkedContent))
		{
			_icon.sprite = reward.GetIcon();
		}
		_background.Set((int)reward.GetRarity());
		int num = reward.Count();
		_amount.SetActive(num > 1);
		_amount.text = $"{num}";
		CardLinkedContent cardLinkedContent = reward as CardLinkedContent;
		if (cardLinkedContent == null)
		{
			if (!(reward is LootboxLinkedContent lootboxLinkedContent))
			{
				SkinLinkedContent skinLinkedContent = reward as SkinLinkedContent;
				if (skinLinkedContent == null)
				{
					if (!(reward is MergeItemLinkedContent mergeItemLinkedContent))
					{
						if (!(reward is DecorationLinkedContent decorationLinkedContent))
						{
							if (!(reward is CurrencySpecialLinkedContent currencySpecialLinkedContent))
							{
								if (!(reward is CurrencyLinkedContent currencyLinkedContent))
								{
									if (reward is BoosterLinkedContent boosterLinkedContent)
									{
										_dropType = MultiDropType.Booster;
										_background.Set((num > 1) ? 11 : 10);
										_bonusType = boosterLinkedContent.BonusType;
									}
								}
								else
								{
									_dropType = MultiDropType.Currency;
									_background.Set((num > 1) ? 11 : 10);
									_currencyType = currencyLinkedContent.Currency;
									_currencyId = currencyLinkedContent.CompositeIdentificator[0];
								}
							}
							else
							{
								_dropType = MultiDropType.Currency;
								_background.Set((num > 1) ? 11 : 10);
								_currencyType = currencySpecialLinkedContent.Currency;
								_currencyId = currencySpecialLinkedContent.CompositeIdentificator[0];
							}
						}
						else
						{
							_dropType = MultiDropType.Decoration;
							_background.Set((num > 1) ? 11 : 10);
							_decorationId = decorationLinkedContent.ID;
						}
					}
					else
					{
						_dropType = MultiDropType.MergeItem;
						_background.Set((num > 1) ? 11 : 10);
						_spawnerCollectionKey = mergeItemLinkedContent.GameItemConfig.Key.Collection;
						_isSpawner = mergeItemLinkedContent.GameItemConfig.HasModule(GIModuleType.ClickSpawn);
					}
				}
				else
				{
					_dropType = MultiDropType.Skin;
					_fakeAssetService.SetFakeSkinIcon(skinLinkedContent.Skin, _icon, (Skin _) => skinLinkedContent.GetFullIcon());
					_skinId = skinLinkedContent.Skin.ID;
				}
			}
			else
			{
				_dropType = MultiDropType.Lootbox;
				_lootboxRarity = lootboxLinkedContent.GetRarity();
				_background.Set((num > 1) ? 11 : 10);
			}
		}
		else
		{
			_dropType = MultiDropType.Card;
			_fakeAssetService.SetFakeCharacterBankImages(cardLinkedContent.Card, _icon, (ICharacter _) => cardLinkedContent.GetFullIcon());
			_girlId = cardLinkedContent.Card.ID;
		}
		UpdateLocalizationKey();
	}
}
