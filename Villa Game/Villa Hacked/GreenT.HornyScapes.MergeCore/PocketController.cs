using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.Data;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.External.StripClub._Scripts.NewEventScripts;
using GreenT.HornyScapes.GameItems;
using GreenT.Types;
using Merge;
using StripClub.NewEvent.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

[MementoHolder]
public class PocketController : Controller<PocketController>, ISavableState
{
	[Serializable]
	public class ItemDictionary : SerializableDictionary<int, string>
	{
	}

	[Serializable]
	public class PocketMemento : Memento
	{
		[SerializeField]
		public Queue<GIData> PocketItemsQueue { get; set; } = new Queue<GIData>();


		[SerializeField]
		public Queue<GIData> PocketEventItemsQueue { get; set; } = new Queue<GIData>();


		public PocketMemento(PocketController pocketController)
			: base(pocketController)
		{
		}
	}

	private PocketMemento pocketData;

	private EventStateService _stateService;

	private EventProvider _provider;

	private GameItemConfigManager _gameItemConfigManager;

	private readonly Subject<PocketMemento> onClear = new Subject<PocketMemento>();

	private readonly Subject<GIData> onItemAdd = new Subject<GIData>();

	private readonly Subject<GIData> onItemRemove = new Subject<GIData>();

	private ContentSelectorGroup contentSelectorGroup;

	public Queue<GIData> PocketItemsQueue => pocketData.PocketItemsQueue;

	public Queue<GIData> PocketEventItemsQueue { get; private set; }

	public IObservable<GIData> OnItemAdd => Observable.AsObservable<GIData>((IObservable<GIData>)onItemAdd);

	public IObservable<PocketMemento> OnClear => Observable.AsObservable<PocketMemento>((IObservable<PocketMemento>)onClear);

	public IObservable<GIData> OnItemRemove => Observable.AsObservable<GIData>((IObservable<GIData>)onItemRemove);

	private static GameItemController _field => Controller<GameItemController>.Instance;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public GIData ItemToPop()
	{
		if (!TryGetQueue(out var queue))
		{
			return null;
		}
		return queue.Peek();
	}

	public bool NotEmpty()
	{
		return !IsEmpty();
	}

	public bool IsEmpty()
	{
		return CountItem() == 0;
	}

	public int CountItem()
	{
		if (!TryGetQueue(out var queue))
		{
			return 0;
		}
		return queue.Count;
	}

	[Inject]
	private void InnerInit(ContentSelectorGroup contentSelectorGroup, EventStateService stateService, EventProvider provider, GameItemConfigManager gameItemConfigManager)
	{
		_stateService = stateService;
		_provider = provider;
		this.contentSelectorGroup = contentSelectorGroup;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public override void Preload()
	{
		base.Preload();
		if (pocketData == null)
		{
			pocketData = new PocketMemento(this);
		}
	}

	public void AddItemToQueue(GIData item, ContentType contentType)
	{
		Queue<GIData> queue = null;
		if (TryGetQueueWithType(contentType, ref queue))
		{
			queue.Enqueue(item);
			onItemAdd.OnNext(item);
		}
	}

	public void AddItemToQueue(GIKey value)
	{
		AddItemToQueue(new GIData(value));
	}

	public void AddItemToQueue(GIData item)
	{
		GIConfig configOrNull = _gameItemConfigManager.GetConfigOrNull(item.Key);
		GetItemQueue(configOrNull.ContentType).Enqueue(item);
		onItemAdd.OnNext(item);
	}

	private Queue<GIData> GetItemQueue(ContentType contentType)
	{
		if (contentType != 0)
		{
			return _provider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.Queue;
		}
		return pocketData.PocketItemsQueue;
	}

	public void RebaseMain(IEnumerable<GIData> newCollection)
	{
		pocketData.PocketItemsQueue.Clear();
		onClear.OnNext(pocketData);
		foreach (GIData item in newCollection)
		{
			pocketData.PocketItemsQueue.Enqueue(item);
			onItemAdd.OnNext(item);
		}
	}

	public void RebaseEvent(IEnumerable<GIData> newCollection)
	{
		Queue<GIData> queue = _provider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.Queue;
		queue.Clear();
		onClear.OnNext(pocketData);
		foreach (GIData item in newCollection)
		{
			queue.Enqueue(item);
			onItemAdd.OnNext(item);
		}
	}

	public void ClearDataInPocket()
	{
		if (TryGetQueue(out var queue))
		{
			queue.Clear();
			onClear.OnNext(pocketData);
		}
	}

	public void DeleteItemInMainPocket(GIData itemToRemove)
	{
		pocketData.PocketItemsQueue = new Queue<GIData>(pocketData.PocketItemsQueue.Where((GIData item) => item != itemToRemove));
	}

	public void DeleteItemInEventPocket(GIData itemToRemove)
	{
		_provider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.DeleteItem(itemToRemove);
	}

	private bool TryGetQueue(out Queue<GIData> queue)
	{
		queue = null;
		if (contentSelectorGroup.Current == ContentType.Main)
		{
			queue = pocketData.PocketItemsQueue;
		}
		else if (contentSelectorGroup.Current == ContentType.Event && _stateService.HaveActiveEvent)
		{
			queue = _provider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.Queue;
		}
		return queue != null;
	}

	private bool TryGetQueueWithType(ContentType contentType, ref Queue<GIData> queue)
	{
		switch (contentType)
		{
		case ContentType.Main:
			queue = pocketData.PocketItemsQueue;
			break;
		case ContentType.Event:
			if (_stateService.HaveActiveEvent)
			{
				queue = _provider.CurrentCalendarProperty.Value.Item2.Data.PocketRepository.Queue;
				break;
			}
			return false;
		}
		return true;
	}

	public void AtButtonClick()
	{
		if (_field.TryGetFirstEmptyPoint(out var pnt) && TryGetQueue(out var queue) && queue.Count != 0)
		{
			GIData gIData = queue.Dequeue().Copy().SetCoordinates(pnt);
			onItemRemove.OnNext(gIData);
		}
	}

	public string UniqueKey()
	{
		return "PocketController";
	}

	public Memento SaveState()
	{
		return pocketData;
	}

	public void LoadState(Memento memento)
	{
		PocketMemento pocketMemento = (PocketMemento)memento;
		if (pocketData == null)
		{
			pocketData = new PocketMemento(this);
		}
		MigrationToBattlePass.Migrate(pocketMemento);
		PocketEventItemsQueue = pocketMemento.PocketEventItemsQueue;
		pocketMemento.PocketEventItemsQueue = null;
		pocketData.PocketItemsQueue = pocketMemento.PocketItemsQueue ?? new Queue<GIData>();
	}
}
