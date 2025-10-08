using StripClub.NewEvent.Save;

namespace StripClub.NewEvent.Data;

public interface IEventDataSaver
{
	void Add(IEventSavableState savableState);
}
