using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Model.Shop.UI;

public class DropOptionView : MonoView<DropSettings>
{
	public class Manager : ViewManager<DropOptionView>
	{
	}

	[SerializeField]
	private Image background;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text count;

	[SerializeField]
	private Sprite miscellaneousBackground;

	[SerializeField]
	private CurrencySpriteDictionary currencySprites;

	private GameSettings _gameSettings;

	private IMergeIconProvider _iconProvider;

	private GameItemConfigManager _gameItemConfigManager;

	[Inject]
	private void Init(GameSettings gameSettings, IMergeIconProvider iconProvider, GameItemConfigManager gameItemConfigManager)
	{
		_gameSettings = gameSettings;
		_iconProvider = iconProvider;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public override void Set(DropSettings drop)
	{
		base.Set(drop);
		count.text = drop.Quantity.ToString();
		Sprite sprite = miscellaneousBackground;
		switch (drop.Type)
		{
		case RewType.Resource:
		{
			CurrencyType currency = ((CurrencySelector)drop.Selector).Currency;
			icon.sprite = _gameSettings.CurrencySettings[currency, default(CompositeIdentificator)].Sprite;
			sprite = currencySprites[currency];
			break;
		}
		case RewType.MergeItem:
		{
			int iD = ((SelectorByID)drop.Selector).ID;
			_gameItemConfigManager.TryGetConfig(iD, out var giConfig);
			icon.sprite = _iconProvider.GetSprite(giConfig.Key);
			break;
		}
		}
		background.sprite = sprite;
	}
}
