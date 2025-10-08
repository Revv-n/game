using System;
using System.Collections.Generic;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MergeCore;
using GreenT.Types;
using Merge;
using StripClub.Messenger.Data;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger.Data;

public class MessageOptionsFactory
{
	private readonly IMergeIconProvider _iconManager;

	private readonly GameSettings _gameSettings;

	private readonly ICurrencyProcessor _currencyProcessor;

	private readonly GameItemConfigManager _gameItemConfigManager;

	public MessageOptionsFactory(IMergeIconProvider iconManager, GameSettings gameSettings, ICurrencyProcessor currencyProcessor, GameItemConfigManager gameItemConfigManager)
	{
		_iconManager = iconManager;
		_gameSettings = gameSettings;
		_currencyProcessor = currencyProcessor;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public List<ResponseOption> CreatePlayerOptions(PlayerMessageConfigMapper.ItemMapper[][] itemMappers, int dialogueID, int number)
	{
		if (itemMappers.Length == 0)
		{
			throw new ArgumentException("Count of player reply options must be greater than 0");
		}
		List<ResponseOption> list = new List<ResponseOption>();
		for (int i = 0; i != itemMappers.Length; i++)
		{
			List<IItemLot> necessaryItems = CreatItemLots(itemMappers, i);
			ResponseOption item = new ResponseOption(dialogueID, number, i, necessaryItems);
			list.Add(item);
		}
		return list;
	}

	private List<IItemLot> CreatItemLots(PlayerMessageConfigMapper.ItemMapper[][] itemMappers, int i)
	{
		List<IItemLot> list = new List<IItemLot>();
		for (int j = 0; j != itemMappers[i].Length; j++)
		{
			Selector selector = SelectorTools.CreateSelector(itemMappers[i][j].ID);
			if (!(selector is SelectorByID selectorByID))
			{
				if (!(selector is CurrencySelector selector2))
				{
					throw new ArgumentOutOfRangeException("Can't create Selector for : " + itemMappers[i][j].ID);
				}
				CreatCurrencyItemLot(itemMappers[i][j], list, selector2);
			}
			else
			{
				CreatMergeItemLot(itemMappers[i][j], list, selectorByID.ID);
			}
		}
		return list;
	}

	private void CreatCurrencyItemLot(PlayerMessageConfigMapper.ItemMapper itemMapper, List<IItemLot> itemNeccessaryForResponse, CurrencySelector selector)
	{
		CurrencyType currency = selector.Currency;
		CompositeIdentificator identificator = selector.Identificator;
		int count = itemMapper.Count;
		if (_gameSettings.CurrencySettings.TryGetValue(currency, out var currencySettings, identificator))
		{
			Sprite alternativeSprite = currencySettings.AlternativeSprite;
			IObservable<Sprite> updateAlternativeSprite = _gameSettings.CurrencySettings[currency, identificator].ObserveEveryValueChanged((CurrencySettings actualSettings) => actualSettings.AlternativeSprite);
			CurrencyItemLot currencyItemLot = new CurrencyItemLot(currency, identificator, count, alternativeSprite, _currencyProcessor);
			currencyItemLot.Initialization(updateAlternativeSprite);
			itemNeccessaryForResponse.Add(currencyItemLot);
		}
	}

	private void CreatMergeItemLot(PlayerMessageConfigMapper.ItemMapper itemMapper, List<IItemLot> itemNeccessaryForResponse, int itemID)
	{
		if (_gameItemConfigManager.TryGetConfig(itemID, out var giConfig))
		{
			GIKey key = giConfig.Key;
			int count = itemMapper.Count;
			Sprite sprite = _iconManager.GetSprite(key);
			MergeItemLot item = new MergeItemLot(key, count, sprite);
			itemNeccessaryForResponse.Add(item);
		}
	}
}
