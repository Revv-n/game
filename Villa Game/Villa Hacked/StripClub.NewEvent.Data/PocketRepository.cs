using System;
using System.Collections.Generic;
using System.Linq;
using Merge;
using StripClub.NewEvent.Save;

namespace StripClub.NewEvent.Data;

public class PocketRepository : IEventSavableState, IDisposableEventInformation, IDisposable
{
	[Serializable]
	public class PocketEventMemento : EventMemento
	{
		public Queue<GIData> PocketItemsQueue;

		public PocketEventMemento(PocketRepository pocketRepository)
			: base(pocketRepository)
		{
			PocketItemsQueue = pocketRepository.pocketItemsQueue;
		}
	}

	private Queue<GIData> pocketItemsQueue = new Queue<GIData>();

	public Queue<GIData> Queue => pocketItemsQueue;

	public void Add(GIData data)
	{
		pocketItemsQueue.Enqueue(data);
	}

	public bool TryGet(out GIData data)
	{
		data = null;
		if (pocketItemsQueue == null || pocketItemsQueue.Count == 0)
		{
			return false;
		}
		data = pocketItemsQueue.Dequeue();
		return true;
	}

	public void DeleteItem(GIData itemToRemove)
	{
		pocketItemsQueue = new Queue<GIData>(pocketItemsQueue.Where((GIData item) => item != itemToRemove));
	}

	public void SetQueue(Queue<GIData> queue)
	{
		pocketItemsQueue = queue;
	}

	public string UniqueKey()
	{
		return "pocket_repository";
	}

	public EventMemento SaveState()
	{
		return new PocketEventMemento(this);
	}

	public void LoadState(EventMemento memento)
	{
		PocketEventMemento pocketEventMemento = (PocketEventMemento)memento;
		if (pocketEventMemento.PocketItemsQueue != null || pocketItemsQueue == null)
		{
			pocketItemsQueue = pocketEventMemento.PocketItemsQueue;
		}
	}

	public void Dispose()
	{
		pocketItemsQueue?.Clear();
	}
}
