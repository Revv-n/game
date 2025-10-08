using System.Collections.Generic;
using GreenT.HornyScapes.GameItems;
using Merge;

namespace GreenT.HornyScapes.MergeCore;

public class InfoGetItem
{
	private GameItemConfigManager _gameItemConfigManager;

	public Dictionary<GIKey, List<GIConfig>> HowToGetDict = new Dictionary<GIKey, List<GIConfig>>(4096);

	public Dictionary<GIConfig, ModuleConfigs.ClickSpawn> Spawners = new Dictionary<GIConfig, ModuleConfigs.ClickSpawn>(1024);

	public Dictionary<GIConfig, ModuleConfigs.Stack> Stacks = new Dictionary<GIConfig, ModuleConfigs.Stack>(64);

	public void Initialize(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
		IEnumerable<GIConfig> collection = _gameItemConfigManager.Collection;
		FillSpawners(collection);
		FillStacks(collection);
		FillHowToGet(collection);
	}

	private void FillSpawners(IEnumerable<GIConfig> configs)
	{
		Spawners.Clear();
		foreach (GIConfig config in configs)
		{
			if (config.TryGetModule<ModuleConfigs.ClickSpawn>(out var result))
			{
				Spawners.Add(config, result);
			}
		}
	}

	private void FillStacks(IEnumerable<GIConfig> configs)
	{
		Stacks.Clear();
		foreach (GIConfig config in configs)
		{
			if (config.TryGetModule<ModuleConfigs.Stack>(out var result))
			{
				Stacks.Add(config, result);
			}
		}
	}

	private void FillHowToGet(IEnumerable<GIConfig> configs)
	{
		HowToGetDict.Clear();
		foreach (GIConfig key in Spawners.Keys)
		{
			foreach (WeightNode<GIData> item in Spawners[key].SpawnPool)
			{
				FillHowToGetItem(key, item);
			}
		}
		foreach (KeyValuePair<GIConfig, ModuleConfigs.Stack> stack in Stacks)
		{
			if (!stack.Value.IsSwapActiveModule || stack.Value.SwapModuleType != GIModuleType.Mixer || stack.Key.UniqId == 121201)
			{
				continue;
			}
			foreach (RecipeModel item2 in stack.Value.ItemsPool)
			{
				foreach (WeightNode<GIData> item3 in item2.Result)
				{
					FillHowToGetItem(stack.Key, item3);
				}
			}
		}
	}

	private void FillHowToGetItem(GIConfig module, WeightNode<GIData> item)
	{
		string collection = item.value.Key.Collection;
		List<GIConfig> collection2 = _gameItemConfigManager.GetCollection(collection);
		if (collection2.Count == 0)
		{
			return;
		}
		for (int i = 0; i < collection2.Count; i++)
		{
			GIKey key = collection2[i].Key;
			if (!HowToGetDict.ContainsKey(key))
			{
				HowToGetDict[key] = new List<GIConfig>(8);
			}
			List<GIConfig> list = HowToGetDict[key];
			if (!list.Contains(module))
			{
				list.Add(module);
			}
		}
	}
}
