using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Constants;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.GameItems;
using Merge;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class SellController : Controller<SellController>, IActionModuleController, IModuleController
{
	[SerializeField]
	private SellConfirmationWindow sellConfirmationWindow;

	private CollectCurrencyServiceForFly _collectCurrencyService;

	private List<string> _importantCollections;

	private ContentSelectorGroup _contentSelectorGroup;

	private GameItemConfigManager gameItemConfigManager;

	private IConstants<string> _constants;

	public override int InitOrder => Priority.Low;

	private GameItemController Field => Controller<GameItemController>.Instance;

	GIModuleType IModuleController.ModuleType => GIModuleType.Sell;

	private CurrencyType GetPriceType
	{
		get
		{
			if (_contentSelectorGroup.Current != 0)
			{
				return CurrencyType.Event;
			}
			return CurrencyType.Soft;
		}
	}

	public event Action<GameItem> OnSellImportant;

	[Inject]
	public void Init(CollectCurrencyServiceForFly collectCurrencyService, ContentSelectorGroup contentSelectorGroup, GameItemConfigManager gameItemConfigManager, IConstants<string> constants)
	{
		_collectCurrencyService = collectCurrencyService;
		_contentSelectorGroup = contentSelectorGroup;
		this.gameItemConfigManager = gameItemConfigManager;
		_constants = constants;
	}

	public override void Preload()
	{
		base.Preload();
		Field.OnItemCreated += AtItemCreated;
		Field.OnItemTakenFromSomethere += AtItemCreated;
	}

	public override void Init()
	{
		ParseImportantCollections();
	}

	public bool IsImportantItem(GIKey key)
	{
		return _importantCollections.Contains(key.Collection.ToLower().Trim(' '));
	}

	private void AtSell(GIBox.Sell sellBox)
	{
		int price = sellBox.Config.Price;
		Field.RemoveItem(sellBox.Parent);
		_collectCurrencyService.Collect(sellBox.Parent, GetPriceType, price, CurrencyAmplitudeAnalytic.SourceType.MergeSell);
	}

	private void AtItemCreated(GameItem item)
	{
		if (item.Config.TryGetModule<ModuleConfigs.Sell>(out var result))
		{
			GIBox.Sell box = new GIBox.Sell(null, result);
			item.AddBox(box);
		}
	}

	void IActionModuleController.ExecuteAction(GIBox.Base box)
	{
		GIBox.Sell sellBox = box as GIBox.Sell;
		if (sellBox == null)
		{
			return;
		}
		GameItem gi = sellBox.Parent;
		if (IsImportantItem(gi.Key))
		{
			sellConfirmationWindow.Setup(sellBox, delegate
			{
				AtSell(sellBox);
				this.OnSellImportant?.Invoke(gi);
			});
			sellConfirmationWindow.Open();
		}
		else
		{
			AtSell(sellBox);
			Controller<SelectionController>.Instance.ClearSelection();
		}
	}

	private void ParseImportantCollections()
	{
		_importantCollections = new List<string>();
		List<string> list = (from x in _constants["not_important_item_sell"].Split(',', StringSplitOptions.None)
			select x.ToLower().Trim(' ')).ToList();
		foreach (GIConfig item2 in gameItemConfigManager.Collection)
		{
			string item = item2.Key.Collection.ToLower().Trim(' ');
			if (!list.Contains(item) && !_importantCollections.Contains(item) && (item2.HasModule(GIModuleType.ClickSpawn) || item2.HasModule(GIModuleType.Stack)))
			{
				_importantCollections.Add(item);
			}
		}
	}
}
