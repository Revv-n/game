using System.Collections.Generic;
using GreenT.Multiplier;
using Zenject;

namespace Merge;

public class ModifyController
{
	[Inject]
	private MultiplierManager multiplierManager;

	public int CalcMaxAmount(GIBox.ClickSpawn box)
	{
		return CalcMaxAmount(box.Config.MaxAmount, box.Parent.Config);
	}

	public int CalcMaxAmount(GIBox.AutoSpawn box)
	{
		return CalcMaxAmount(box.Config.MaxAmount, box.Parent.Config);
	}

	public int CalcMaxAmount(int maxAmount, GameItem item)
	{
		return CalcMaxAmount(maxAmount, item.Config);
	}

	public int CalcMaxAmount(int maxAmount, GIConfig config)
	{
		IMultiplier multiplier = (config.NotAffectedAll ? GetMaxAmountWithoutTotalMultiplier(config.UniqId) : GetMaxAmountTotalMultiplier(config.UniqId));
		return CalcMaxAmount(maxAmount, multiplier);
	}

	private int CalcMaxAmount(int maxAmount, IMultiplier multiplier)
	{
		return (int)((double)maxAmount + multiplier.Factor.Value);
	}

	private IMultiplier GetMaxAmountTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerMaxAmountMultipliers.TotalByKey(uniqId);
	}

	private IMultiplier GetMaxAmountWithoutTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerMaxAmountMultipliers.GetMultiplier(uniqId);
	}

	public float RestoreTime(GIBox.ClickSpawn box)
	{
		return RestoreTime(box.Config.RestoreTime, box.Parent.Config);
	}

	public float RestoreTime(GIBox.AutoSpawn box)
	{
		return RestoreTime(box.Config.RestoreTime, box.Parent.Config);
	}

	public float RestoreTime(GIBox.Mixer box)
	{
		return RestoreTime(box.ModifiedMaxMixingTime, box.Parent.Config);
	}

	public float RestoreTime(float restoreTime, GameItem item)
	{
		return RestoreTime(restoreTime, item.Config);
	}

	public float RestoreTime(float restoreTime, GIConfig config)
	{
		IMultiplier multiplier = (config.NotAffectedAll ? GetReloadWithoutTotalMultiplier(config.UniqId) : GetReloadTotalMultiplier(config.UniqId));
		return RestoreTime(restoreTime, multiplier);
	}

	private float RestoreTime(float restoreTime, IMultiplier multiplier)
	{
		return restoreTime * (float)multiplier.Factor.Value;
	}

	private IMultiplier GetReloadTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerReloadMultipliers.TotalByKey(uniqId);
	}

	private IMultiplier GetReloadWithoutTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerReloadMultipliers.GetMultiplier(uniqId);
	}

	public List<WeightNode<GIData>> RefreshModifySpawnPool(GIBox.AutoSpawn box)
	{
		return RefreshModifySpawnPool(box.ModifiedSpawnPool, box.Config.SpawnPool, box.Parent.Config);
	}

	public List<WeightNode<GIData>> RefreshModifySpawnPool(GIBox.ClickSpawn box)
	{
		return RefreshModifySpawnPool(box.ModifiedSpawnPool, box.Config.SpawnPool, box.Parent.Config);
	}

	public List<WeightNode<GIData>> RefreshModifySpawnPool(GIBox.Mixer box)
	{
		return RefreshModifySpawnPool(box.ModifiedMixPool, box.ActiveRecipe.Result, box.Parent.Config);
	}

	public List<WeightNode<GIData>> RefreshModifySpawnPool(List<WeightNode<GIData>> modifiedSpawnPool, List<WeightNode<GIData>> spawnPool, GIConfig config)
	{
		IMultiplier multiplier = (config.NotAffectedAll ? GetProductionWithoutTotalMultiplier(config.UniqId) : GetProductionTotalMultiplier(config.UniqId));
		return RefreshModifySpawnPool(modifiedSpawnPool, spawnPool, multiplier);
	}

	private List<WeightNode<GIData>> RefreshModifySpawnPool(List<WeightNode<GIData>> modifiedSpawnPool, List<WeightNode<GIData>> sourceSpawnPool, IMultiplier multiplier)
	{
		for (int i = 0; i < modifiedSpawnPool.Count; i++)
		{
			modifiedSpawnPool[i].weight = sourceSpawnPool[i].weight + (float)multiplier.Factor.Value;
		}
		return modifiedSpawnPool;
	}

	private IMultiplier GetProductionTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerProductionMultipliers.TotalByKey(uniqId);
	}

	private IMultiplier GetProductionWithoutTotalMultiplier(int uniqId)
	{
		return multiplierManager.SpawnerProductionMultipliers.GetMultiplier(uniqId);
	}
}
