using System;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Events;

public class EventItemsRewardCard : EventRewardCard, IDisposable
{
	[SerializeField]
	private SpriteStates _lootboxStatable;

	[SerializeField]
	private Sprite _bpRewardSprite;

	[SerializeField]
	private Image icon;

	private IDisposable _iconChangeStream;

	public override void Set(EventReward source)
	{
		base.Set(source);
		LinkedContent content = source.Content;
		if (!(content is CurrencyLinkedContent currencyLinkedContent))
		{
			if (!(content is LootboxLinkedContent lootboxLinkedContent))
			{
				if (content is BattlePassLevelLinkedContent _)
				{
					HandleBattlePassLinkedContent(_);
				}
				else
				{
					icon.sprite = source.Content.GetIcon();
				}
			}
			else
			{
				HandleLootboxContent(lootboxLinkedContent);
			}
		}
		else
		{
			HandleCurrencyContent(currencyLinkedContent);
		}
	}

	private void HandleLootboxContent(LootboxLinkedContent lootboxLinkedContent)
	{
		_lootboxStatable.Set((int)lootboxLinkedContent.GetRarity());
	}

	private void HandleBattlePassLinkedContent(BattlePassLevelLinkedContent _)
	{
		icon.sprite = _bpRewardSprite;
	}

	private void HandleCurrencyContent(CurrencyLinkedContent currencyLinkedContent)
	{
		CurrencySettings currencySettings = _settings.CurrencySettings[currencyLinkedContent.Currency, currencyLinkedContent.CompositeIdentificator];
		ApplyData(currencySettings.AlternativeSprite, currencyLinkedContent.Currency, currencyLinkedContent.CompositeIdentificator);
		_iconChangeStream = currencySettings.ObserveEveryValueChanged((CurrencySettings actualSettings) => actualSettings.AlternativeSprite).Subscribe(delegate(Sprite sprite)
		{
			ApplyData(sprite, currencyLinkedContent.Currency, currencyLinkedContent.CompositeIdentificator);
		});
		void ApplyData(Sprite sprite, CurrencyType currencyType, CompositeIdentificator currencyIdentificator)
		{
			icon.sprite = ((sprite == null) ? _settings.CurrencyPlaceholder[currencyType, currencyIdentificator].AlternativeSprite : sprite);
		}
	}

	public void Dispose()
	{
		_iconChangeStream?.Dispose();
	}
}
