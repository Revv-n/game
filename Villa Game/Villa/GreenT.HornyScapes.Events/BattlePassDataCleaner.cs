using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.MergeCore;

namespace GreenT.HornyScapes.Events;

public class BattlePassDataCleaner : ICalendarDataCleaner
{
	private readonly ISaver _saver;

	private readonly BattlePassSettingsProvider _battlePassSettingsProvider;

	private readonly MergePointsController _mergePointsController;

	private readonly ResetAfterBundleLotController _resetAfterBundleLotController;

	private BattlePassDataCleaner(ISaver saver, BattlePassSettingsProvider battlePassSettingsProvider, MergePointsController mergePointsController, ResetAfterBundleLotController resetAfterBundleLotController)
	{
		_saver = saver;
		_battlePassSettingsProvider = battlePassSettingsProvider;
		_mergePointsController = mergePointsController;
		_resetAfterBundleLotController = resetAfterBundleLotController;
	}

	public void CleanData(CalendarModel calendarModel)
	{
		if (calendarModel == null || !_battlePassSettingsProvider.TryGetBattlePass(calendarModel.BalanceId, out var battlePass) || battlePass?.Data == null)
		{
			return;
		}
		try
		{
			CleanBattlePassData(battlePass.Data, calendarModel);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void CleanBattlePassData(BattlePassDataCase data, CalendarModel calendar)
	{
		data.MergedCurrencyData.Reset();
		_saver.Remove(data.MergedCurrencyData);
		data.LevelInfo.Reset();
		data.LevelInfo.Delete();
		data.StartData.SetPremiumPurchased(value: false);
		data.StartData.Delete();
		_mergePointsController.TryRemoveEventPoints(EventStructureType.BattlePass);
		_resetAfterBundleLotController.TryClear(EventStructureType.BattlePass, calendar.EventMapper.ID);
	}
}
