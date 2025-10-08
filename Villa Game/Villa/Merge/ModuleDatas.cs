using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.Model;
using UnityEngine;

namespace Merge;

public static class ModuleDatas
{
	[Serializable]
	public abstract class Base
	{
		public abstract GIModuleType ModuleType { get; }

		public abstract Base Copy();
	}

	[Serializable]
	public class AutoUpgrade : Base, ITimerData
	{
		[SerializeField]
		private RefTimer upgradeTimer = TimeMaster.DefaultTimer;

		public override GIModuleType ModuleType => GIModuleType.AutoUpgrade;

		public RefTimer MainTimer
		{
			get
			{
				return upgradeTimer;
			}
			set
			{
				upgradeTimer = value;
			}
		}

		public override Base Copy()
		{
			return new AutoUpgrade
			{
				upgradeTimer = upgradeTimer.Copy()
			};
		}
	}

	[Serializable]
	public class AutoSpawn : Base, ITimerData
	{
		[SerializeField]
		private RefTimer spawnTimer = TimeMaster.DefaultTimer;

		[SerializeField]
		private bool timerActive;

		[SerializeField]
		private int contains;

		[SerializeField]
		private SpeedUpUnitData speedupData = new SpeedUpUnitData();

		public override GIModuleType ModuleType => GIModuleType.AutoSpawn;

		public RefTimer MainTimer
		{
			get
			{
				return spawnTimer;
			}
			set
			{
				spawnTimer = value;
			}
		}

		public int Amount
		{
			get
			{
				return contains;
			}
			set
			{
				contains = value;
				if (value == 0)
				{
					this.OnIsEmpty?.Invoke();
				}
			}
		}

		public bool TimerActive
		{
			get
			{
				return timerActive;
			}
			set
			{
				timerActive = value;
			}
		}

		public SpeedUpUnitData SpeedupData
		{
			get
			{
				return speedupData;
			}
			set
			{
				speedupData = value;
			}
		}

		public event Action OnIsEmpty;

		public override Base Copy()
		{
			return new AutoSpawn
			{
				contains = contains,
				spawnTimer = spawnTimer.Copy(),
				timerActive = timerActive
			};
		}

		public RefSkipableTimer GetMainTimer()
		{
			if (!(spawnTimer is RefSkipableTimer))
			{
				spawnTimer = new RefSkipableTimer(spawnTimer);
			}
			return spawnTimer as RefSkipableTimer;
		}
	}

	[Serializable]
	public class ClickSpawn : Base, ITimerData
	{
		[SerializeField]
		private RefTimer spawnTimer = TimeMaster.DefaultTimer;

		[SerializeField]
		private bool timerActive;

		[SerializeField]
		private int contains;

		[SerializeField]
		private List<GIData> dropQueue;

		[SerializeField]
		private SpeedUpUnitData speedupData = new SpeedUpUnitData();

		public override GIModuleType ModuleType => GIModuleType.ClickSpawn;

		public RefTimer MainTimer
		{
			get
			{
				return spawnTimer;
			}
			set
			{
				spawnTimer = value;
			}
		}

		public int Amount
		{
			get
			{
				return contains;
			}
			set
			{
				contains = value;
				if (value == 0)
				{
					this.OnIsEmpty?.Invoke();
				}
			}
		}

		public bool WasRefreshedOffline { get; set; }

		public bool TimerActive
		{
			get
			{
				return timerActive;
			}
			set
			{
				timerActive = value;
			}
		}

		public List<GIData> DropQueue
		{
			get
			{
				return dropQueue;
			}
			set
			{
				dropQueue = value;
			}
		}

		public SpeedUpUnitData SpeedupData
		{
			get
			{
				return speedupData;
			}
			set
			{
				speedupData = value;
			}
		}

		public event Action OnIsEmpty;

		[field: NonSerialized]
		public event Action OnTimerComplete;

		public override Base Copy()
		{
			return new ClickSpawn
			{
				contains = contains,
				spawnTimer = spawnTimer.Copy(),
				timerActive = timerActive,
				dropQueue = ((dropQueue == null) ? null : dropQueue.Select((GIData x) => x.Copy()).ToList())
			};
		}

		public RefSkipableTimer GetMainTimer()
		{
			if (!(spawnTimer is RefSkipableTimer))
			{
				spawnTimer = new RefSkipableTimer(spawnTimer);
			}
			return spawnTimer as RefSkipableTimer;
		}

		public void InvokeOnTimerComplete()
		{
			this.OnTimerComplete?.Invoke();
		}
	}

	[Serializable]
	public class Chest : Base, ITimerData
	{
		[SerializeField]
		private RefTimer openTimer = TimeMaster.DefaultTimer;

		[SerializeField]
		private bool isOpeningNow;

		[SerializeField]
		private bool opened;

		[SerializeField]
		private SpeedUpUnitData speedupData = new SpeedUpUnitData();

		public override GIModuleType ModuleType => GIModuleType.Chest;

		public RefTimer MainTimer
		{
			get
			{
				return GetMainTimer();
			}
			set
			{
				openTimer = value;
			}
		}

		public bool IsOpeningNow
		{
			get
			{
				return isOpeningNow;
			}
			set
			{
				isOpeningNow = value;
			}
		}

		public bool AlreadyOpened
		{
			get
			{
				return opened;
			}
			set
			{
				opened = value;
			}
		}

		public SpeedUpUnitData SpeedupData
		{
			get
			{
				return speedupData;
			}
			set
			{
				speedupData = value;
			}
		}

		public override Base Copy()
		{
			return new Chest
			{
				openTimer = openTimer.Copy(),
				isOpeningNow = isOpeningNow,
				opened = opened
			};
		}

		public RefSkipableTimer GetMainTimer()
		{
			if (!(openTimer is RefSkipableTimer))
			{
				openTimer = new RefSkipableTimer(openTimer);
			}
			return openTimer as RefSkipableTimer;
		}
	}

	[Serializable]
	public class Bubble : Base, ITimerData
	{
		[SerializeField]
		private RefTimer lifeTime = TimeMaster.DefaultTimer;

		[SerializeField]
		private GIData inBubble = new GIData();

		[SerializeField]
		private int openPrice;

		public override GIModuleType ModuleType => GIModuleType.Bubble;

		public RefTimer MainTimer
		{
			get
			{
				return lifeTime;
			}
			set
			{
				lifeTime = value;
			}
		}

		public GIData Rest
		{
			get
			{
				return inBubble;
			}
			set
			{
				inBubble = value;
			}
		}

		public int OpenPrice
		{
			get
			{
				return openPrice;
			}
			set
			{
				openPrice = value;
			}
		}

		public override Base Copy()
		{
			return new Bubble
			{
				lifeTime = lifeTime.Copy(),
				inBubble = inBubble.Copy(),
				openPrice = openPrice
			};
		}
	}

	[Serializable]
	public class Locked : Base
	{
		[SerializeField]
		private int strength = 1;

		[SerializeField]
		private bool blocksWisible;

		[SerializeField]
		private int unlockPrice;

		[SerializeField]
		private int randomSkin;

		public override GIModuleType ModuleType => GIModuleType.Locked;

		public int Strength
		{
			get
			{
				return strength;
			}
			set
			{
				strength = value;
			}
		}

		public bool BlocksMerge
		{
			get
			{
				return blocksWisible;
			}
			set
			{
				blocksWisible = value;
			}
		}

		public int UnlockPrice
		{
			get
			{
				return unlockPrice;
			}
			set
			{
				unlockPrice = value;
			}
		}

		public int RandomSkin
		{
			get
			{
				return randomSkin;
			}
			set
			{
				randomSkin = value;
			}
		}

		public override Base Copy()
		{
			return new Locked
			{
				strength = strength,
				blocksWisible = blocksWisible,
				unlockPrice = unlockPrice
			};
		}
	}

	[Serializable]
	public class Tesla : Base
	{
		[SerializeField]
		private RefTimer lifeTimer = TimeMaster.DefaultTimer;

		[SerializeField]
		private bool activated;

		public override GIModuleType ModuleType => GIModuleType.Tesla;

		public RefTimer LifeTimer
		{
			get
			{
				return lifeTimer;
			}
			set
			{
				lifeTimer = value;
			}
		}

		public bool Activated
		{
			get
			{
				return activated;
			}
			set
			{
				activated = value;
			}
		}

		public override Base Copy()
		{
			return new Tesla
			{
				LifeTimer = LifeTimer.Copy(),
				Activated = Activated
			};
		}
	}

	[Serializable]
	public class Mixer : Base, ITimerData
	{
		public enum StateMixer : uint
		{
			None,
			Wait,
			Mixing,
			Spawn
		}

		[SerializeField]
		private RefTimer mixingTimer = TimeMaster.DefaultTimer;

		[SerializeField]
		private StateMixer currentState = StateMixer.Wait;

		[SerializeField]
		private int contains;

		[SerializeField]
		private SpeedUpUnitData speedupData = new SpeedUpUnitData();

		[SerializeField]
		private int? activeRecipeID;

		public override GIModuleType ModuleType => GIModuleType.Mixer;

		public RefTimer MainTimer
		{
			get
			{
				return GetMainTimer();
			}
			set
			{
				mixingTimer = value;
			}
		}

		public StateMixer CurrentState
		{
			get
			{
				return currentState;
			}
			set
			{
				currentState = value;
			}
		}

		public int Amount
		{
			get
			{
				return contains;
			}
			set
			{
				contains = value;
			}
		}

		public int? ActiveRecipeID
		{
			get
			{
				return activeRecipeID;
			}
			set
			{
				activeRecipeID = value;
				if (activeRecipeID.HasValue)
				{
					this.AddActiveRecipeID?.Invoke();
				}
			}
		}

		public SpeedUpUnitData SpeedupData
		{
			get
			{
				return speedupData;
			}
			set
			{
				speedupData = value;
			}
		}

		public event Action OnIsEmpty;

		public event Action AddActiveRecipeID;

		public override Base Copy()
		{
			return new Mixer
			{
				mixingTimer = mixingTimer.Copy(),
				currentState = currentState,
				contains = contains,
				activeRecipeID = activeRecipeID
			};
		}

		public RefSkipableTimer GetMainTimer()
		{
			if (!(mixingTimer is RefSkipableTimer))
			{
				mixingTimer = new RefSkipableTimer(mixingTimer);
			}
			return mixingTimer as RefSkipableTimer;
		}

		public void ChangeValue(int value)
		{
			contains = value;
		}
	}

	[Serializable]
	public class Stack : Base
	{
		[SerializeField]
		private List<WeightNode<GIData>> items = new List<WeightNode<GIData>>();

		public override GIModuleType ModuleType => GIModuleType.Stack;

		public List<WeightNode<GIData>> Items
		{
			get
			{
				return items;
			}
			set
			{
				items = value;
			}
		}

		public override Base Copy()
		{
			List<WeightNode<GIData>> list = new List<WeightNode<GIData>>();
			if (items != null && items.Count > 0)
			{
				foreach (WeightNode<GIData> item in items)
				{
					list.Add(new WeightNode<GIData>(item.value.Copy(), item.Weight));
				}
			}
			return new Stack
			{
				items = list
			};
		}
	}

	[Serializable]
	public class Special : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Special;

		public override Base Copy()
		{
			return new Special();
		}
	}

	[Serializable]
	public class Collect : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Collect;

		public override Base Copy()
		{
			return new Collect();
		}
	}

	[Serializable]
	public class Merge : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Merge;

		public override Base Copy()
		{
			return new Merge();
		}
	}

	[Serializable]
	public class Sell : Base
	{
		public override GIModuleType ModuleType => GIModuleType.Sell;

		public override Base Copy()
		{
			return new Sell();
		}
	}

	[Serializable]
	public class MergePoints : Base
	{
		[SerializeField]
		private CompositeIdentificator identificator;

		[SerializeField]
		private int count;

		[SerializeField]
		private CurrencyType currencyType;

		public override GIModuleType ModuleType => GIModuleType.MergePoints;

		public CompositeIdentificator Identificator => identificator;

		public CurrencyType CurrencyType => currencyType;

		public int Count => count;

		public MergePoints(CompositeIdentificator identificator, int count, CurrencyType currencyType)
		{
			this.identificator = identificator;
			this.count = count;
			this.currencyType = currencyType;
		}

		public override Base Copy()
		{
			return new MergePoints(identificator, count, currencyType);
		}
	}
}
