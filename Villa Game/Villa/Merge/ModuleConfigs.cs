using System;
using System.Collections.Generic;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;

namespace Merge;

public static class ModuleConfigs
{
	[Serializable]
	public abstract class Base
	{
		public abstract GIModuleType ModuleType { get; }

		public Base()
		{
		}
	}

	[Serializable]
	public class Collect : Base
	{
		[Serializable]
		public abstract class CollectedParametres
		{
		}

		[Serializable]
		public class CurrencyParams : CollectedParametres
		{
			[SerializeField]
			private CurrencyType currency;

			[SerializeField]
			private int amount;

			[SerializeField]
			private CompositeIdentificator compositeIdentificator;

			public CompositeIdentificator CompositeIdentificator => compositeIdentificator;

			public int Amount => amount;

			public CurrencyType CurrencyType => currency;

			public CurrencyParams()
			{
			}

			public CurrencyParams(CurrencyType currency, int amount, CompositeIdentificator compositeIdentificator)
			{
				this.currency = currency;
				this.amount = amount;
				this.compositeIdentificator = compositeIdentificator;
			}

			public CurrencyParams(CurrencyType currency, int amount)
			{
				this.currency = currency;
				this.amount = amount;
				compositeIdentificator = default(CompositeIdentificator);
			}
		}

		[Serializable]
		public class SpeedUpParams : CollectedParametres
		{
			[SerializeField]
			private float value;

			public float Value => value;

			public SpeedUpParams()
			{
			}

			public SpeedUpParams(float value)
			{
				this.value = value;
			}
		}

		[SerializeField]
		private CollectableType type;

		[SerializeField]
		private CollectedParametres parametres;

		public override GIModuleType ModuleType => GIModuleType.Collect;

		public CollectableType CollectableType => type;

		public CollectedParametres Parametres => parametres;

		public bool TryGetCurrencyParams(out CurrencyParams currencyParams)
		{
			currencyParams = Parametres as CurrencyParams;
			return currencyParams != null;
		}

		public bool TryGetTimeBoostParams(out SpeedUpParams speedUpParams)
		{
			speedUpParams = Parametres as SpeedUpParams;
			return speedUpParams != null;
		}

		public Collect()
		{
		}

		public Collect(CurrencyType currency, int amount)
		{
			type = CollectableType.Currency;
			parametres = new CurrencyParams(currency, amount);
		}

		public Collect(CurrencyType currency, int amount, CompositeIdentificator compositeIdentificator)
		{
			type = CollectableType.Currency;
			parametres = new CurrencyParams(currency, amount, compositeIdentificator);
		}

		public Collect(CollectableType collectableType, CollectedParametres parametres)
		{
			type = collectableType;
			this.parametres = parametres;
		}
	}

	[Serializable]
	public class Merge : Base
	{
		[SerializeField]
		private GIData mergeResult = new GIData();

		[SerializeField]
		private List<GIData> bonus = new List<GIData>();

		[SerializeField]
		private float bonusChance;

		[SerializeField]
		private List<WeightNode<GIData>> bonusMergeResult = new List<WeightNode<GIData>>();

		public override GIModuleType ModuleType => GIModuleType.Merge;

		public GIData MergeResult => mergeResult;

		public float BonusChance => bonusChance;

		public List<WeightNode<GIData>> BubbleMergeList => bonusMergeResult;

		public List<GIData> Bonus => bonus;

		public Merge()
		{
		}

		public Merge(GIData mergeResult, List<GIData> bonus, float bonusChance, List<WeightNode<GIData>> bonusMergeResult)
		{
			this.mergeResult = mergeResult;
			this.bonus = bonus;
			this.bonusChance = bonusChance;
			this.bonusMergeResult = bonusMergeResult;
		}
	}

	[Serializable]
	public class Sell : Base
	{
		[SerializeField]
		private int price;

		public override GIModuleType ModuleType => GIModuleType.Sell;

		public int Price => price;

		public Sell()
		{
		}

		public Sell(int price)
		{
			this.price = price;
		}
	}

	[Serializable]
	public class AutoUpgrade : Base
	{
		[SerializeField]
		private GIData target = new GIData();

		[SerializeField]
		private float time;

		public override GIModuleType ModuleType => GIModuleType.AutoUpgrade;

		public GIData Target => target;

		public float Time => time;

		public AutoUpgrade()
		{
		}

		public AutoUpgrade(GIData target, float time)
		{
			this.target = target;
			this.time = time;
		}
	}

	[Serializable]
	public class AutoSpawn : Base
	{
		[SerializeField]
		private float restoreTime;

		[SerializeField]
		private int restoreAmount;

		[SerializeField]
		private int maxAmount;

		[SerializeField]
		private float secPrice;

		[SerializeField]
		private List<WeightNode<GIData>> spawnPool = new List<WeightNode<GIData>>();

		[SerializeField]
		private GIDestroyType destroyType;

		[SerializeField]
		private GIData destroyResult = new GIData();

		public override GIModuleType ModuleType => GIModuleType.AutoSpawn;

		public int MaxAmount => maxAmount;

		public bool CanRestore
		{
			get
			{
				if (restoreAmount != 0)
				{
					return restoreTime != 0f;
				}
				return false;
			}
		}

		public int RestoreAmount => restoreAmount;

		public float RestoreTime => restoreTime;

		public GIDestroyType DestroyType => destroyType;

		public GIData DestroyResult => destroyResult;

		public List<WeightNode<GIData>> SpawnPool => spawnPool;

		public float SecPrice => secPrice;

		public AutoSpawn()
		{
		}

		public AutoSpawn(float restoreTime, int restoreAmount, int maxAmount, float secPrice, List<WeightNode<GIData>> spawnPool, GIDestroyType destroyType, GIData destroyResult)
		{
			this.restoreTime = restoreTime;
			this.restoreAmount = restoreAmount;
			this.maxAmount = maxAmount;
			this.spawnPool = spawnPool;
			this.destroyType = destroyType;
			this.destroyResult = destroyResult;
			this.secPrice = secPrice;
		}
	}

	[Serializable]
	public class ClickSpawn : Base
	{
		[SerializeField]
		private float restoreTime;

		[SerializeField]
		private float speedUpMul = 1f;

		[SerializeField]
		private int restoreAmount;

		[SerializeField]
		private int maxAmount;

		[SerializeField]
		private int energyPrice;

		[SerializeField]
		private GIDestroyType destroyType;

		[SerializeField]
		private GIData destroyResult = new GIData();

		[SerializeField]
		private List<WeightNode<GIData>> spawnPool = new List<WeightNode<GIData>>();

		public override GIModuleType ModuleType => GIModuleType.ClickSpawn;

		public List<WeightNode<GIData>> SpawnPool => spawnPool;

		public int MaxAmount => maxAmount;

		public bool CanRestore
		{
			get
			{
				if (restoreAmount != 0)
				{
					return restoreTime != 0f;
				}
				return false;
			}
		}

		public int RestoreAmount => restoreAmount;

		public float RestoreTime => restoreTime;

		public int EnergyPrice => energyPrice;

		public GIDestroyType DestroyType => destroyType;

		public GIData DestroyResult => destroyResult;

		public float SpeedUpMul => speedUpMul;

		public ClickSpawn()
		{
		}

		public ClickSpawn(float restoreTime, float speedUpMul, int restoreAmount, int maxAmount, int energyPrice, GIDestroyType destroyType, GIData destroyResult, List<WeightNode<GIData>> spawnPool)
		{
			this.restoreTime = restoreTime;
			this.speedUpMul = speedUpMul;
			this.restoreAmount = restoreAmount;
			this.maxAmount = maxAmount;
			this.energyPrice = energyPrice;
			this.destroyType = destroyType;
			this.destroyResult = destroyResult;
			this.spawnPool = spawnPool;
		}
	}

	[Serializable]
	public class Chest : Base
	{
		[SerializeField]
		private bool openable;

		[SerializeField]
		private float timeToOpen;

		[SerializeField]
		private float priceMul = 1f;

		public override GIModuleType ModuleType => GIModuleType.Chest;

		public float TimeToOpen => timeToOpen;

		public float PriceMul => priceMul;

		public bool IsOpenable => openable;

		public Chest()
		{
		}

		public Chest(bool openable, float timeToOpen, float priceMul)
		{
			this.openable = openable;
			this.timeToOpen = timeToOpen;
			this.priceMul = priceMul;
		}
	}

	[Serializable]
	public class Tesla : Base
	{
		[SerializeField]
		private float lifeTime;

		[SerializeField]
		private float multiplier = 2f;

		[SerializeField]
		private GIDestroyType destroyType;

		[SerializeField]
		private GIData destroyResult = new GIData();

		public float LifeTime => lifeTime;

		public float Multiplier => multiplier;

		public GIDestroyType DestroyType => destroyType;

		public GIData DestroyResult => destroyResult;

		public override GIModuleType ModuleType => GIModuleType.Tesla;

		public Tesla()
		{
		}

		public Tesla(float lifeTime, float multiplier, GIDestroyType destroyType, GIData destroyResult)
		{
			this.lifeTime = lifeTime;
			this.multiplier = multiplier;
			this.destroyType = destroyType;
			this.destroyResult = destroyResult;
		}
	}

	[Serializable]
	public class Mixer : Base
	{
		[SerializeField]
		private GIDestroyType destroyType;

		[SerializeField]
		private GIData destroyResult = new GIData();

		[SerializeField]
		private int energyPrice;

		public override GIModuleType ModuleType => GIModuleType.Mixer;

		public GIDestroyType DestroyType => destroyType;

		public GIData DestroyResult => destroyResult;

		public int Energy => energyPrice;

		public Mixer(GIDestroyType destroyType, GIData destroyResult, int energyPrice)
		{
			this.destroyType = destroyType;
			this.energyPrice = energyPrice;
			this.destroyResult = destroyResult;
		}
	}

	[Serializable]
	public class Stack : Base
	{
		[SerializeField]
		private bool isSwapActiveModule;

		[SerializeField]
		private GIModuleType swapModuleType = GIModuleType.Stack;

		private List<RecipeModel> itemsPool = new List<RecipeModel>();

		public override GIModuleType ModuleType => GIModuleType.Stack;

		public List<RecipeModel> ItemsPool => itemsPool;

		public bool IsSwapActiveModule => isSwapActiveModule;

		public GIModuleType SwapModuleType => swapModuleType;

		public Stack()
		{
		}

		public Stack(List<RecipeModel> itemsPool, bool isSwapActiveModule, GIModuleType swapModuleType)
		{
			this.itemsPool = itemsPool;
			this.isSwapActiveModule = isSwapActiveModule;
			this.swapModuleType = swapModuleType;
		}
	}

	[Serializable]
	public class MergePoints : Base
	{
		[Serializable]
		public class MergePointsCreateData
		{
			public readonly CurrencySelector CurrencySelector;

			public readonly int PointsChance;

			public readonly int PointsQty;

			public readonly UnlockType[] LockerTypes;

			public string[] LockerValue;

			[NonSerialized]
			public ILocker Locker;

			public MergePointsCreateData(CurrencySelector currencySelector, int pointsChance, int pointsQty, UnlockType[] lockerTypes, string[] lockerValue)
			{
				CurrencySelector = currencySelector;
				PointsQty = pointsQty;
				LockerTypes = lockerTypes;
				LockerValue = lockerValue;
				PointsChance = pointsChance;
			}

			public void SetLocker(ILocker locker)
			{
				Locker = locker;
			}
		}

		[SerializeField]
		private MergePointsCreateData[] createData;

		public override GIModuleType ModuleType => GIModuleType.MergePoints;

		public MergePointsCreateData[] CreateData => createData;

		public MergePoints()
		{
		}

		public MergePoints(MergePointsCreateData[] createData)
		{
			this.createData = createData;
		}
	}

	[Serializable]
	public class MergeShop : Base
	{
		[SerializeField]
		private string priceResource;

		[SerializeField]
		private int price;

		[SerializeField]
		private int[] sale;

		[SerializeField]
		private string shopSection;

		public override GIModuleType ModuleType => GIModuleType.MergeShop;

		public string PriceResource => priceResource;

		public int Price => price;

		public int[] Sale => sale;

		public string ShopSection => shopSection;

		public MergeShop()
		{
		}

		public MergeShop(string priceResource, int price, int[] sale, string shopSection)
		{
			this.priceResource = priceResource;
			this.price = price;
			this.sale = sale;
			this.shopSection = shopSection;
		}
	}

	[Serializable]
	public class Bubble : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Bubble;
	}

	[Serializable]
	public class Locked : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Locked;
	}
}
