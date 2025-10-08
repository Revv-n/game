using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventActivityConfigHandler : BaseMiniEventConfigHandler
{
	private readonly ActivitiesQuestManager _activitiesQuestManager;

	private readonly ActivitiesShopManager _activitiesShopManager;

	private readonly MiniEventTaskManager _miniEventTaskManager;

	private readonly ISaver _saver;

	public MiniEventActivityConfigHandler(ActivitiesQuestManager activitiesQuestManager, ActivitiesShopManager activitiesShopManager, MiniEventTabsManager miniEventTabsManager, MiniEventTaskManager miniEventTaskManager, MiniEventActivityTabAdministrator miniEventActivityTabAdministrator, ConfigType configType, ISaver saver)
		: base(configType, miniEventTabsManager, miniEventActivityTabAdministrator)
	{
		_activitiesQuestManager = activitiesQuestManager;
		_activitiesShopManager = activitiesShopManager;
		_miniEventTaskManager = miniEventTaskManager;
		_saver = saver;
	}

	public override IEnumerable<IController> Handle<TMapper>(TMapper mapper, int eventId, int activityId, int calendarId)
	{
		ActivityMapper activityMapper = mapper as ActivityMapper;
		IEnumerable<IController> enumerable = HandleAny(calendarId, eventId, activityId, activityMapper.quest_tabs, activityMapper.quest_tabs_priority, TabType.Task);
		IEnumerable<IController> enumerable2 = HandleAny(calendarId, eventId, activityId, activityMapper.shop_tabs, activityMapper.shop_tabs_priority, TabType.Shop);
		if (enumerable2 == null)
		{
			return enumerable;
		}
		if (enumerable == null)
		{
			return enumerable2;
		}
		return enumerable.Concat(enumerable2).ToArray();
	}

	private IEnumerable<IController> HandleAny(int calendarId, int miniEventId, int activityId, int[] tabs, int[] tabsPriority, TabType tabType)
	{
		if (!tabs.Any())
		{
			return null;
		}
		List<IController> list = new List<IController>();
		for (int i = 0; i < tabs.Length; i++)
		{
			CompositeIdentificator eventIdentificator = new CompositeIdentificator(miniEventId, activityId);
			CompositeIdentificator tabIdentificator = new CompositeIdentificator(tabs[i]);
			CompositeIdentificator datadentificator = new CompositeIdentificator(GetActivityDataId(tabType, tabs[i]));
			string iconBundleKey = GetIconBundleKey(tabs[i], tabType);
			string backgroundBundleKey = GetBackgroundBundleKey(tabs[i], tabType);
			MiniEventActivityTab miniEventActivityTab = CreateActivityTab(calendarId, tabsPriority[i], iconBundleKey, backgroundBundleKey, eventIdentificator, tabIdentificator, datadentificator, tabType);
			AdministrateActivityTab(miniEventActivityTab);
			AddMiniEventActivityTab(miniEventActivityTab);
			if (tabType == TabType.Task)
			{
				List<MiniEventTask> tasks = _miniEventTaskManager.Tasks.Where((MiniEventTask _task) => _task.Identificator == datadentificator).ToList();
				list.Add(new MiniEventTasksController(tasks, _saver));
			}
		}
		return list;
	}

	private int GetActivityDataId(TabType tabType, int tabId)
	{
		int result = 0;
		switch (tabType)
		{
		case TabType.Task:
			result = _activitiesQuestManager.GetActivityInfo(tabId).quest_massive_id;
			break;
		case TabType.Shop:
			result = _activitiesShopManager.GetActivityInfo(tabId).bank_tab_id;
			break;
		}
		return result;
	}

	protected override string GetIconBundleKey(int id, TabType tabType)
	{
		string result = null;
		switch (tabType)
		{
		case TabType.Task:
			result = _activitiesQuestManager.Collection.FirstOrDefault((ActivityQuestMapper quest) => quest.tab_id == id).icon;
			break;
		case TabType.Shop:
			result = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper shop) => shop.tab_id == id).icon;
			break;
		}
		return result;
	}

	protected override string GetBackgroundBundleKey(int id, TabType tabType)
	{
		string result = null;
		switch (tabType)
		{
		case TabType.Task:
			result = string.Empty;
			break;
		case TabType.Shop:
			result = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper shop) => shop.tab_id == id).shop_bg;
			break;
		}
		return result;
	}
}
