using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Info.UI;
using GreenT.HornyScapes.MergeCore;
using Merge.Core.Windows;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Collection;

public class CollectionWindow : PopupWindow
{
	[SerializeField]
	private CollectionWindowItem prefab;

	[SerializeField]
	private Transform parent;

	[SerializeField]
	private int columns = 4;

	[SerializeField]
	private Image iconItem;

	[SerializeField]
	private ContainerWindowGrid grid;

	[SerializeField]
	private LocalizedTextMeshPro namelbl;

	[SerializeField]
	private LocalizedTextMeshPro levelLbl;

	[SerializeField]
	private List<ContainerWindowItem> items;

	[SerializeField]
	private InfoBonusesView bonusesView;

	private SmartPool<CollectionWindowItem> pool;

	private CompareGIData compareGIData;

	public void Init()
	{
		compareGIData = new CompareGIData();
		pool = new SmartPool<CollectionWindowItem>(prefab, parent).ActivateDefaultTransformValidation();
	}

	public CollectionWindow SetCollection(CollectionController.CollectionMemento.CGroup group, GIConfig gIConfig, bool needTopPart, bool needShowAllCollection = false)
	{
		bonusesView.Init(gIConfig);
		pool.ReturnAllItems();
		CollectionWindowItem collectionWindowItem = null;
		int num = 0;
		foreach (CollectionController.CollectionMemento.CItem item in group.Items)
		{
			CollectionWindowItem collectionWindowItem2 = pool.Pop();
			collectionWindowItem2.SetCItem(item, needShowAllCollection);
			collectionWindowItem2.transform.SetAsLastSibling();
			if (collectionWindowItem != null)
			{
				collectionWindowItem.SetArrowVisible(++num % columns != 0).SetArrowActive(item.Opened && collectionWindowItem.Node.Opened);
			}
			collectionWindowItem = collectionWindowItem2;
		}
		levelLbl.SetArguments(gIConfig.Level);
		namelbl.Init(gIConfig.GameItemName);
		grid.gameObject.SetActive(needTopPart);
		if (needTopPart)
		{
			items.ForEach(delegate(ContainerWindowItem x)
			{
				x.gameObject.SetActive(value: false);
			});
			ModuleConfigs.ClickSpawn module = gIConfig.GetModule<ModuleConfigs.ClickSpawn>();
			ModuleConfigs.AutoSpawn module2 = gIConfig.GetModule<ModuleConfigs.AutoSpawn>();
			List<GIData> list = new List<GIData>();
			if (module != null)
			{
				list = module.SpawnPool.Select((WeightNode<GIData> x) => x.value).OrderBy((GIData x) => x, compareGIData).ToList();
				for (int i = 0; i < Mathf.Min(list.Count, items.Count); i++)
				{
					items[i].SetItem(list[i], ClockAnabled: false).SetActive(active: true);
				}
			}
			if (module2 != null)
			{
				list.Clear();
				list = module2.SpawnPool.Select((WeightNode<GIData> x) => x.value).OrderBy((GIData x) => x, compareGIData).ToList();
				for (int j = module?.SpawnPool.Count ?? 0; j < Mathf.Min(list.Count + (module?.SpawnPool.Count ?? 0), items.Count); j++)
				{
					items[j].SetItem(list[j - (module?.SpawnPool.Count ?? 0)], ClockAnabled: true).SetActive(active: true);
				}
			}
		}
		iconItem.sprite = IconProvider.GetGISprite(gIConfig.Key);
		collectionWindowItem.SetArrowVisible(visible: false);
		return this;
	}
}
