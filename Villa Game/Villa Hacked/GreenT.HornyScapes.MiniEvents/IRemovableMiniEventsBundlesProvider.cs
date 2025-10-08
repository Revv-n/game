namespace GreenT.HornyScapes.MiniEvents;

public interface IRemovableMiniEventsBundlesProvider
{
	void TryRemove(int miniEventId);

	void TryRemove(int tabId, TabType tabType);
}
