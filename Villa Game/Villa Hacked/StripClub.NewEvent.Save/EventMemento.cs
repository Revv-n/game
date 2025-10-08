using System;

namespace StripClub.NewEvent.Save;

[Serializable]
public abstract class EventMemento
{
	public string UniqueKey { get; private set; }

	public EventMemento(IEventSavableState pocketRepository)
	{
		UniqueKey = pocketRepository.UniqueKey();
	}
}
