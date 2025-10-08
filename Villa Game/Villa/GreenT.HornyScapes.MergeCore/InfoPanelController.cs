using System.Collections.Generic;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore.UI;
using Merge;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class InfoPanelController : Controller<InfoPanelController>, IMasterController
{
	[SerializeField]
	private InfoPanel view;

	private GIData savedSelledItem;

	private GameItem currentItem;

	private Dictionary<GIModuleType, IActionModuleController> actionControllers = new Dictionary<GIModuleType, IActionModuleController>();

	[Inject]
	private ICurrencyProcessor currencyProcessor;

	[Inject]
	private ContentSelectorGroup contentSelectorGroup;

	[Inject]
	private GameItemConfigManager gameItemConfigManager;

	private GameItemController Field => Controller<GameItemController>.Instance;

	public override void Preload()
	{
		base.Preload();
		Controller<SelectionController>.Instance.OnSelectionChange += AtSelectionChange;
		view.OnAction += AtAction;
		view.OnUnsell += AtUnsell;
	}

	public override void Init()
	{
		Controller<SellController>.Instance.OnSellImportant += AtSellImportant;
	}

	private void AtUnsell()
	{
		if (Field.IsFull)
		{
			Pocket.AddItemToQueue(savedSelledItem);
			int price = gameItemConfigManager.GetConfigOrNull(savedSelledItem.Key).GetModule<ModuleConfigs.Sell>().Price;
			currencyProcessor.TrySpent((contentSelectorGroup.Current != 0) ? CurrencyType.Event : CurrencyType.Soft, price);
			savedSelledItem = null;
			view.Set(null);
		}
		else
		{
			currentItem = Controller<GameItemController>.Instance.CreateItem(savedSelledItem);
			currentItem.DoCreate();
			int price2 = currentItem.Config.GetModule<ModuleConfigs.Sell>().Price;
			currencyProcessor.TrySpent((contentSelectorGroup.Current != 0) ? CurrencyType.Event : CurrencyType.Soft, price2);
			savedSelledItem = null;
			view.Set(currentItem);
		}
	}

	private void AtAction(GIBox.Base box)
	{
		actionControllers[box.ModuleType].ExecuteAction(box);
		if (box.ModuleType == GIModuleType.Sell && !Controller<SellController>.Instance.IsImportantItem(box.Parent.Key))
		{
			CacheSelledItem(box.Parent.Data.Copy());
		}
		else
		{
			view.Set(Controller<GameItemController>.Instance.CurrentField.Field[box.Parent.Coordinates]);
		}
	}

	private void AtSellImportant(GameItem gi)
	{
		CacheSelledItem(gi.Data.Copy());
	}

	private void CacheSelledItem(GIData copy)
	{
		savedSelledItem = copy;
	}

	private void AtSelectionChange(GameItem selection)
	{
		Select(selection);
		savedSelledItem = null;
		UnsubscribeItem(currentItem);
		SubscribeItem(selection);
	}

	private void Select(GameItem selection)
	{
		currentItem = selection;
		view.Set(currentItem);
	}

	private void SubscribeItem(GameItem selection)
	{
		if (!(selection == null))
		{
			selection.OnRemoving += UnsubscribeItem;
		}
	}

	private void UnsubscribeItem(GameItem certainItem)
	{
		if (!(certainItem == null))
		{
			certainItem.OnRemoving -= UnsubscribeItem;
		}
	}

	void IMasterController.LinkControllers(IList<BaseController> controllers)
	{
		foreach (BaseController controller in controllers)
		{
			if (controller is IActionModuleController)
			{
				actionControllers.Add((controller as IActionModuleController).ModuleType, controller as IActionModuleController);
			}
		}
	}
}
