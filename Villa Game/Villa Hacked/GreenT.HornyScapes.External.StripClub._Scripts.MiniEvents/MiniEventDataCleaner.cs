using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MiniEvents;
using StripClub.Model;
using StripClub.Model.Data;

namespace GreenT.HornyScapes.External.StripClub._Scripts.MiniEvents;

public class MiniEventDataCleaner : ICalendarDataCleaner
{
	private readonly ISaver _saver;

	private readonly MiniEventSettingsProvider _miniEventSettingsProvider;

	private readonly Currencies _mainBalance;

	private readonly MiniEventFieldCleaner _fieldCleaner;

	private readonly MiniEventMergeItemsCleanService _mergeItemsCleanService;

	private readonly MiniEventTabsManager _miniEventTabsManager;

	private readonly MiniEventActivityTabAdministrator _miniEventActivityTabAdministrator;

	public MiniEventDataCleaner(ISaver saver, MiniEventSettingsProvider miniEventSettingsProvider, Currencies mainBalance, MiniEventFieldCleaner miniEventFieldCleaner, MiniEventMergeItemsCleanService mergeItemsCleanService, MiniEventTabsManager miniEventTabsManager, MiniEventActivityTabAdministrator miniEventActivityTabAdministrator)
	{
		_saver = saver;
		_mainBalance = mainBalance;
		_miniEventSettingsProvider = miniEventSettingsProvider;
		_fieldCleaner = miniEventFieldCleaner;
		_mergeItemsCleanService = mergeItemsCleanService;
		_miniEventTabsManager = miniEventTabsManager;
		_miniEventActivityTabAdministrator = miniEventActivityTabAdministrator;
	}

	public void CleanData(CalendarModel calendarModel)
	{
		MiniEvent minievent = _miniEventSettingsProvider.Collection.FirstOrDefault((MiniEvent minievent) => minievent.EventId == calendarModel.BalanceId && minievent.CalendarId == calendarModel.UniqID);
		if (minievent == null)
		{
			return;
		}
		IEnumerable<MiniEventActivityTab> tabs = _miniEventTabsManager.Collection.Where((MiniEventActivityTab tab) => tab.CalendarId == minievent.CalendarId);
		_miniEventActivityTabAdministrator.DisposeTabs(tabs);
		foreach (IController controller in minievent.Controllers)
		{
			controller.RefreshSavables();
		}
		SimpleCurrency simpleCurrency = _mainBalance[CurrencyType.MiniEvent, minievent.CurrencyIdentificator];
		simpleCurrency.Initialize();
		_mergeItemsCleanService.Clean(simpleCurrency.Identificator);
		_fieldCleaner.CleanData(minievent);
		minievent.RefreshSaveState();
		_saver.Delete(simpleCurrency);
		_saver.Delete(minievent);
	}
}
