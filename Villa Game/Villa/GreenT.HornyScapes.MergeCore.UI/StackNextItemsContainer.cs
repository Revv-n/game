using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using Merge;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public class StackNextItemsContainer : MonoBehaviour
{
	[SerializeField]
	private StackNextItemInfo nextItemInfoPrefab;

	[SerializeField]
	private Transform parentItemInfo;

	private MergeIconService _iconProvider;

	private InfoGetItem infoGetItem;

	[Inject]
	public void Init(MergeIconService iconProvider, InfoGetItem infoGetItem)
	{
		_iconProvider = iconProvider;
		this.infoGetItem = infoGetItem;
	}

	public void SetStack(GIBox.Stack stack)
	{
		if (stack.FilterRecipe.Count == 1)
		{
			if (stack.Data.Items.Count > 0)
			{
				foreach (WeightNode<GIData> itemRecipe in stack.FilterRecipe.First().Items)
				{
					WeightNode<GIData> weightNode = stack.Data.Items.FirstOrDefault((WeightNode<GIData> findItem) => findItem.value.Key == itemRecipe.value.Key);
					int count = ((weightNode != null) ? ((int)weightNode.Weight) : 0);
					int max = (int)itemRecipe.Weight;
					Sprite sprite = _iconProvider.GetSprite(itemRecipe.value.Key);
					StackNextItemInfo stackNextItemInfo = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
					stackNextItemInfo.SetIcon(sprite);
					stackNextItemInfo.SetInfo(itemRecipe.value.Key);
					stackNextItemInfo.SetCount(count, max);
				}
				return;
			}
			{
				foreach (WeightNode<GIData> item2 in stack.FilterRecipe.First().Items)
				{
					int count2 = 0;
					int max2 = (int)item2.Weight;
					Sprite sprite2 = _iconProvider.GetSprite(item2.value.Key);
					StackNextItemInfo stackNextItemInfo2 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
					stackNextItemInfo2.SetIcon(sprite2);
					stackNextItemInfo2.SetInfo(item2.value.Key);
					stackNextItemInfo2.SetCount(count2, max2);
				}
				return;
			}
		}
		List<WeightNode<GIData>> list = stack.FilterRecipe.First().Items;
		foreach (RecipeModel item3 in stack.FilterRecipe)
		{
			list = list.Intersect(item3.Items, new WeightNodeWithPartialMatchComparer()).ToList();
		}
		if (list.Count > 0)
		{
			foreach (WeightNode<GIData> itemRecipe in list)
			{
				WeightNode<GIData> weightNode2 = stack.Data.Items.FirstOrDefault((WeightNode<GIData> findItem) => findItem.value.Key == itemRecipe.value.Key);
				int count3 = ((weightNode2 != null) ? ((int)weightNode2.Weight) : 0);
				int max3 = (int)itemRecipe.Weight;
				Sprite sprite3 = _iconProvider.GetSprite(itemRecipe.value.Key);
				StackNextItemInfo stackNextItemInfo3 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
				stackNextItemInfo3.SetIcon(sprite3);
				stackNextItemInfo3.SetInfo(itemRecipe.value.Key);
				stackNextItemInfo3.SetCount(count3, max3);
			}
			{
				foreach (KeyValuePair<GIKey, Sprite> item4 in FindSpawner(stack.nextItems, list.Select((WeightNode<GIData> matchItem) => matchItem.value.Key).ToList()))
				{
					StackNextItemInfo stackNextItemInfo4 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
					stackNextItemInfo4.SetIcon(item4.Value);
					stackNextItemInfo4.SetInfo(item4.Key);
				}
				return;
			}
		}
		if (stack.Data.Items.Count > 0)
		{
			RecipeModel recipeModel = stack.FilterRecipe.First();
			foreach (WeightNode<GIData> item in stack.Data.Items)
			{
				WeightNode<GIData> weightNode3 = recipeModel.Items.FirstOrDefault((WeightNode<GIData> findItem) => findItem.value.Key == item.value.Key);
				int count4 = (int)item.Weight;
				int max4 = ((weightNode3 != null) ? ((int)weightNode3.Weight) : ((int)item.Weight));
				Sprite sprite4 = _iconProvider.GetSprite(item.value.Key);
				StackNextItemInfo stackNextItemInfo5 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
				stackNextItemInfo5.SetIcon(sprite4);
				stackNextItemInfo5.SetInfo(item.value.Key);
				stackNextItemInfo5.SetCount(count4, max4);
			}
			{
				foreach (KeyValuePair<GIKey, Sprite> item5 in FindSpawner(stack.nextItems))
				{
					StackNextItemInfo stackNextItemInfo6 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
					stackNextItemInfo6.SetIcon(item5.Value);
					stackNextItemInfo6.SetInfo(item5.Key);
				}
				return;
			}
		}
		foreach (KeyValuePair<GIKey, Sprite> item6 in FindSpawner(stack.nextItems))
		{
			StackNextItemInfo stackNextItemInfo7 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
			stackNextItemInfo7.SetIcon(item6.Value);
			stackNextItemInfo7.SetInfo(item6.Key);
		}
	}

	public void SetItems(List<GIKey> items, bool isFindSpawner = true)
	{
		Clear();
		if (_iconProvider == null)
		{
			return;
		}
		if (isFindSpawner)
		{
			foreach (KeyValuePair<GIKey, Sprite> item in FindSpawner(items))
			{
				StackNextItemInfo stackNextItemInfo = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
				stackNextItemInfo.SetIcon(item.Value);
				stackNextItemInfo.SetInfo(item.Key);
			}
			return;
		}
		foreach (GIKey item2 in items)
		{
			Sprite sprite = _iconProvider.GetSprite(item2);
			StackNextItemInfo stackNextItemInfo2 = Object.Instantiate(nextItemInfoPrefab, parentItemInfo);
			stackNextItemInfo2.SetIcon(sprite);
			stackNextItemInfo2.SetInfo(item2);
		}
	}

	private Dictionary<GIKey, Sprite> FindSpawner(List<GIKey> items, List<GIKey> ignoreItem = null)
	{
		Dictionary<GIKey, Sprite> dictionary = new Dictionary<GIKey, Sprite>();
		Dictionary<GIKey, List<GIConfig>> dictionary2 = new Dictionary<GIKey, List<GIConfig>>();
		foreach (GIKey item in items)
		{
			if ((ignoreItem == null || !ignoreItem.Contains(item)) && infoGetItem.HowToGetDict.TryGetValue(item, out var value))
			{
				dictionary2.Add(item, value);
			}
		}
		foreach (KeyValuePair<GIKey, List<GIConfig>> item2 in dictionary2)
		{
			using List<GIConfig>.Enumerator enumerator3 = item2.Value.GetEnumerator();
			if (enumerator3.MoveNext())
			{
				GIConfig current2 = enumerator3.Current;
				if (!dictionary.ContainsKey(current2.Key))
				{
					dictionary.Add(current2.Key, _iconProvider.GetSprite(current2.Key));
				}
			}
		}
		return dictionary;
	}

	public void Clear()
	{
		foreach (Transform item in parentItemInfo)
		{
			Object.Destroy(item.gameObject);
		}
	}
}
