using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Tasks;
using GreenT.Types;
using Merge;
using StripClub.Model;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Cheats.UI;

public class AddAllItemsInPocketCheat : MonoBehaviour, IValidatedCheat, ICheat, IContentSelector, ISelector<ContentType>
{
	public class GIKeyComparer : IEqualityComparer<GIKey>
	{
		public bool Equals(GIKey x, GIKey y)
		{
			return string.CompareOrdinal(x.Collection, y.Collection) == 0;
		}

		public int GetHashCode(GIKey obj)
		{
			return obj.Collection.GetHashCode();
		}
	}

	[SerializeField]
	private Button cheatButton;

	private ContentType _currentContent;

	private EventSettingsProvider _eventSettingsProvider;

	private CalendarQueue _calendarQueue;

	private TaskManagerCluster _taskCluster;

	private GameItemConfigManager _gameItemConfigManager;

	[Inject]
	public void Init(CalendarQueue calendarQueue, TaskManagerCluster taskCluster, EventSettingsProvider eventSettingsProvider, GameItemConfigManager gameItemConfigManager)
	{
		_calendarQueue = calendarQueue;
		_taskCluster = taskCluster;
		_eventSettingsProvider = eventSettingsProvider;
		_gameItemConfigManager = gameItemConfigManager;
	}

	private void OnEnable()
	{
		cheatButton.onClick.AddListener(Apply);
	}

	private void OnDisable()
	{
		cheatButton.onClick.RemoveListener(Apply);
	}

	public void Select(ContentType selector)
	{
		_currentContent = selector;
		Validate();
	}

	public bool IsValid()
	{
		return _currentContent == ContentType.Event;
	}

	public void Validate()
	{
		cheatButton.interactable = IsValid();
	}

	public void Apply()
	{
		if (IsValid())
		{
			FulfillPocket();
		}
	}

	public void FulfillPocket()
	{
		GreenT.HornyScapes.Events.Event @event = _eventSettingsProvider.GetEvent(_calendarQueue.GetActiveCalendar(EventStructureType.Event).BalanceId);
		if (@event == null)
		{
			return;
		}
		foreach (GIKey eventItemKey in GetEventItemKeys(@event))
		{
			Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.AddItemToQueue(eventItemKey);
		}
	}

	private List<GIKey> GetEventItemKeys(GreenT.HornyScapes.Events.Event @event)
	{
		IEnumerable<string> keysFromCurrentEventTasks = GetKeysFromCurrentEventTasks();
		List<string> list = @event.Data.MergeField.Field.Select((GameItem _data) => _data.Key.Collection).Union(keysFromCurrentEventTasks).Distinct()
			.ToList();
		List<GIKey> list2 = new List<GIKey>();
		for (int i = 0; i != list.Count; i++)
		{
			string collectionId = list[i];
			IEnumerable<GIKey> collection = from x in _gameItemConfigManager.GetCollection(collectionId)
				select x.Key;
			list2.AddRange(collection);
		}
		List<GIKey> spawners = GetSpawners(list2);
		list2.AddRange(spawners);
		return list2;
	}

	private List<GIKey> GetSpawners(List<GIKey> keys)
	{
		List<GIKey> spawnerKeys = new List<GIKey>();
		Dictionary<string, List<GIConfig>> collections = _gameItemConfigManager.GetCollections();
		List<GIConfig> source = (from _pair in collections
			select _pair.Value.FirstOrDefault((GIConfig _x) => _x.HasModule<ModuleConfigs.ClickSpawn>()) into _config
			where _config != null
			select _config).ToList();
		List<GIConfig> source2 = (from _pair in collections
			select _pair.Value.FirstOrDefault((GIConfig _x) => _x.HasModule<ModuleConfigs.AutoSpawn>()) into _config
			where _config != null
			select _config).ToList();
		GIKey[] array = keys.Distinct(new GIKeyComparer()).ToArray();
		foreach (GIKey key in array)
		{
			GIConfig gIConfig = source.FirstOrDefault((GIConfig _config) => _config.GetModule<ModuleConfigs.ClickSpawn>().SpawnPool.Any((WeightNode<GIData> _item) => _item.value.Key.Equals(key)));
			if (gIConfig != null)
			{
				AddCollectionOfSpawnerToKeys(gIConfig);
				continue;
			}
			gIConfig = source2.FirstOrDefault((GIConfig _config) => _config.GetModule<ModuleConfigs.AutoSpawn>().SpawnPool.Any((WeightNode<GIData> _item) => _item.value.Key.Equals(key)));
			if (gIConfig != null)
			{
				AddCollectionOfSpawnerToKeys(gIConfig);
			}
		}
		return spawnerKeys;
		void AddCollectionOfSpawnerToKeys(GIConfig spawnerConfig)
		{
			IEnumerable<GIKey> collection = (from _config in _gameItemConfigManager.GetCollection(spawnerConfig.Key.Collection)
				select _config.Key).Except(keys);
			spawnerKeys.AddRange(collection);
		}
	}

	private IEnumerable<string> GetKeysFromCurrentEventTasks()
	{
		Task[] array = _taskCluster[_currentContent].Collection.ToArray();
		Task[] array2 = array.Where((Task _task) => _task.IsRewarded || _task.IsActive).ToArray();
		Task[] array3 = array.Except(array2).ToArray();
		List<Task> list = new List<Task>();
		IEnumerable<Task> source = array2.Union(list);
		Task[] array4 = array3;
		foreach (Task task in array4)
		{
			if (task.Lock is CompositeLocker compositeLocker)
			{
				foreach (TaskLocker locker in compositeLocker.Lockers.OfType<TaskLocker>())
				{
					if (source.Any((Task _task) => _task.ID == locker.ItemID))
					{
						list.Add(task);
					}
				}
			}
			else
			{
				ILocker @lock = task.Lock;
				TaskLocker taskLocker1 = @lock as TaskLocker;
				if (taskLocker1 != null && source.Any((Task _task) => _task.ID == taskLocker1.ItemID))
				{
					list.Add(task);
				}
			}
		}
		return source.SelectMany((Task _task) => from _x in _task.Goal.Objectives.OfType<MergeItemObjective>()
			select _x.ItemKey.Collection).Distinct();
	}

	private void LookingForUntrackedKey(GIKey current, List<GIKey> keys)
	{
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(current);
		ModuleConfigs.Merge module = configOrNull.GetModule<ModuleConfigs.Merge>();
		if (module != null)
		{
			GIKey key = module.MergeResult.Key;
			if (!keys.Contains(key))
			{
				keys.Add(key);
			}
		}
		IEnumerable<WeightNode<GIData>> enumerable = new WeightNode<GIData>[0];
		ModuleConfigs.AutoSpawn module2 = configOrNull.GetModule<ModuleConfigs.AutoSpawn>();
		if (module2 != null)
		{
			enumerable = enumerable.Union(module2.SpawnPool);
		}
		ModuleConfigs.ClickSpawn module3 = configOrNull.GetModule<ModuleConfigs.ClickSpawn>();
		if (module3 != null)
		{
			enumerable = enumerable.Union(module3.SpawnPool);
		}
		IEnumerable<GIKey> collection = from _key in enumerable.Select((WeightNode<GIData> x) => x.value.Key).Distinct()
			where !keys.Contains(_key)
			select _key;
		keys.AddRange(collection);
	}
}
