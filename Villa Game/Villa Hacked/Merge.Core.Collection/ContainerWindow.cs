using System.Collections.Generic;
using System.Linq;
using Merge.Core.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Collection;

public class ContainerWindow : PopupWindow
{
	public class SortByGroups : IComparer<GIData>
	{
		public int Compare(GIData x, GIData y)
		{
			if (x.Key.Collection.GetHashCode() > y.Key.Collection.GetHashCode())
			{
				return 1;
			}
			if (x.Key.ID > y.Key.ID)
			{
				return 1;
			}
			if (x.Key.ID == y.Key.ID)
			{
				return 0;
			}
			return -1;
		}
	}

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private ContainerWindowGrid grid;

	[SerializeField]
	private List<ContainerWindowItem> items;

	public GIConfig Config { get; private set; }

	public ContainerWindow SetItem(GIConfig config)
	{
		if (Config == config)
		{
			return this;
		}
		Config = config;
		titleText.text = config.GameItemName;
		icon.sprite = IconProvider.GetGISprite(config.Key);
		items.ForEach(delegate(ContainerWindowItem x)
		{
			x.gameObject.SetActive(value: false);
		});
		ModuleConfigs.ClickSpawn module = config.GetModule<ModuleConfigs.ClickSpawn>();
		grid.SetSizeByItemsCount(module.SpawnPool.Count);
		List<GIData> list = (from x in module.SpawnPool
			orderby x.weight descending
			select x.value).ToList();
		for (int i = 0; i < Mathf.Min(list.Count, items.Count); i++)
		{
			items[i].SetItem(list[i], ClockAnabled: true).SetActive(active: true);
		}
		return this;
	}
}
