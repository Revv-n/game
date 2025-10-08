using System;
using GreenT.Data;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Providers;

namespace GreenT.HornyScapes.Sellouts.Services;

public class SelloutDataCleaner : ICalendarDataCleaner
{
	private readonly SelloutManager _selloutManager;

	private readonly SelloutStateManager _selloutStateManager;

	private readonly MergePointsController _mergePointsController;

	private readonly ResetAfterBundleLotController _resetAfterBundleLotController;

	private readonly ISaver _saver;

	public SelloutDataCleaner(SelloutManager selloutManager, SelloutStateManager selloutStateManager, MergePointsController mergePointsController, ResetAfterBundleLotController resetAfterBundleLotController, ISaver saver)
	{
		_selloutManager = selloutManager;
		_selloutStateManager = selloutStateManager;
		_mergePointsController = mergePointsController;
		_resetAfterBundleLotController = resetAfterBundleLotController;
		_saver = saver;
	}

	public void CleanData(CalendarModel calendarModel)
	{
		if (calendarModel == null || !_selloutManager.TryGetSellout(calendarModel.BalanceId, out var sellout) || sellout == null)
		{
			return;
		}
		try
		{
			CleanSelloutData(sellout, calendarModel);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void CleanSelloutData(Sellout sellout, CalendarModel calendarModel)
	{
		sellout.Reset();
		_saver.Delete(sellout);
		_mergePointsController.TryRemoveEventPoints(EventStructureType.Sellout);
		_resetAfterBundleLotController.TryClear(EventStructureType.Sellout, calendarModel.EventMapper.ID);
		_selloutStateManager.DeactivateSellout(sellout);
	}
}
