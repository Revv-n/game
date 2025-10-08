using System.Collections.Generic;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class BaseMiniEventConfigHandler : IMiniEventConfigHandler
{
	private readonly MiniEventTabsManager _miniEventTabsManager;

	private readonly MiniEventActivityTabAdministrator _miniEventActivityTabAdministrator;

	public ConfigType ConfigType { get; }

	protected BaseMiniEventConfigHandler(ConfigType configType, MiniEventTabsManager miniEventTabsManager, MiniEventActivityTabAdministrator miniEventActivityTabAdministrator)
	{
		ConfigType = configType;
		_miniEventTabsManager = miniEventTabsManager;
		_miniEventActivityTabAdministrator = miniEventActivityTabAdministrator;
	}

	public abstract IEnumerable<IController> Handle<TMapper>(TMapper mapper, int eventId, int activityId, int calendarId);

	protected virtual string GetIconBundleKey(int id, TabType tabType)
	{
		return string.Empty;
	}

	protected virtual string GetBackgroundBundleKey(int id, TabType tabType)
	{
		return string.Empty;
	}

	protected MiniEventActivityTab CreateActivityTab(int calendarId, int priorityId, string iconBundleKey, string backgroundBundleKey, CompositeIdentificator eventIdentificator, CompositeIdentificator tabIdentificator, CompositeIdentificator dataIdentificator, TabType tabType)
	{
		return new MiniEventActivityTab(calendarId, priorityId, iconBundleKey, backgroundBundleKey, eventIdentificator, tabIdentificator, dataIdentificator, tabType);
	}

	protected void AddMiniEventActivityTab(MiniEventActivityTab miniEventActivityTab)
	{
		_miniEventTabsManager.Add(miniEventActivityTab);
	}

	protected void AdministrateActivityTab(MiniEventActivityTab miniEventActivityTab)
	{
		_miniEventActivityTabAdministrator.AdministrateActivityTab(miniEventActivityTab);
	}
}
