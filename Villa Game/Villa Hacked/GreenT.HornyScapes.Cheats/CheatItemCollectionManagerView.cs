using System.Collections.Generic;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Cheats;

public class CheatItemCollectionManagerView : MonoBehaviour
{
	public CheatMergeItemView prefab;

	public Transform AllItemsParent;

	private MergeIconService _iconProvider;

	private InfoGetItem infoGetItem;

	private ModifyController modifyController;

	private GameItemConfigManager gameItemConfigManager;

	private List<CheatMergeItemView> views = new List<CheatMergeItemView>();

	private bool isInit;

	[Inject]
	private void Init(MergeIconService iconProvider, InfoGetItem infoGetItem, ModifyController modifyController, GameItemConfigManager gameItemConfigManager)
	{
		_iconProvider = iconProvider;
		this.infoGetItem = infoGetItem;
		this.modifyController = modifyController;
		this.gameItemConfigManager = gameItemConfigManager;
	}

	public void Initialize()
	{
		if (isInit)
		{
			return;
		}
		foreach (GIConfig item in gameItemConfigManager.Collection)
		{
			CheatMergeItemView cheatMergeItemView = Object.Instantiate(prefab, AllItemsParent);
			cheatMergeItemView.Set(infoGetItem, modifyController);
			cheatMergeItemView.Set(item, _iconProvider.GetSprite(item.Key));
			views.Add(cheatMergeItemView);
		}
		isInit = true;
	}

	public void Refresh()
	{
		foreach (CheatMergeItemView view in views)
		{
			view.Refresh(_iconProvider.GetSprite(view.Source.Key));
		}
	}
}
