using System;
using GreenT.Data;
using GreenT.HornyScapes.BattlePassSpace;

namespace GreenT.HornyScapes.Events;

public class EventBattlePassDataCleaner
{
	private readonly ISaver _saver;

	public EventBattlePassDataCleaner(ISaver saver)
	{
		_saver = saver;
	}

	public void CleanData(BattlePass battlePass)
	{
		if (battlePass == null || battlePass?.Data == null)
		{
			return;
		}
		try
		{
			CleanBattlePassData(battlePass.Data);
		}
		catch (Exception exception)
		{
			exception.LogException();
		}
	}

	private void CleanBattlePassData(BattlePassDataCase data)
	{
		data.MergedCurrencyData.Reset();
		_saver.Remove(data.MergedCurrencyData);
		data.LevelInfo.Reset();
		data.LevelInfo.Delete();
		data.StartData.SetPremiumPurchased(value: false);
		data.StartData.Delete();
	}
}
