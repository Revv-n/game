using System;
using UnityEngine.EventSystems;

namespace Merge;

public static class EventTriggerExtention
{
	public static void AddClickCallback(this EventTrigger et, Action<PointerEventData> action)
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			action((PointerEventData)data);
		});
		et.triggers.Add(entry);
	}

	public static void AddClickCallback(this EventTrigger et, Action action)
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			action();
		});
		et.triggers.Add(entry);
	}

	public static void AddCallback(this EventTrigger et, Action action, EventTriggerType type)
	{
		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = type
		};
		entry.callback.AddListener(delegate
		{
			action();
		});
		et.triggers.Add(entry);
	}
}
