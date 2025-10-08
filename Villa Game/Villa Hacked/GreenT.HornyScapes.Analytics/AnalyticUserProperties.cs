using System.Collections.Generic;
using GreenT.HornyScapes.StarShop;
using StripClub.Model;
using StripClub.Model.Data;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Analytics;

public class AnalyticUserProperties
{
	private readonly User user;

	private readonly PlayerPaymentsStats playerPaymentsStats;

	private readonly StarShopManager starShopManager;

	private readonly TournamentPointsStorage tournamentPointsStorage;

	private readonly ICohortAnalyticConverter cohortAnalyticConverter;

	private readonly IPlayerBasics playerBasics;

	public AnalyticUserProperties(User user, ICurrencyProcessor currencies, PlayerPaymentsStats playerPaymentsStats, StarShopManager starShopManager, TournamentPointsStorage tournamentPointsStorage, CurrencyProcessor currencyProcessor, ICohortAnalyticConverter cohortAnalyticConverter, IPlayerBasics playerBasics)
	{
		this.user = user;
		this.user = user;
		this.playerPaymentsStats = playerPaymentsStats;
		this.starShopManager = starShopManager;
		this.tournamentPointsStorage = tournamentPointsStorage;
		this.cohortAnalyticConverter = cohortAnalyticConverter;
		this.playerBasics = playerBasics;
	}

	public void AddUserPropertiesToAnalyticEvent(AmplitudeEvent analyticsEvent)
	{
		if (user.PlayerID != null)
		{
			analyticsEvent.AddUserProperty("playerId", user.PlayerID);
		}
		analyticsEvent.AddUserProperty("player_power", tournamentPointsStorage.LastPower);
		analyticsEvent.AddUserProperty("step_completed", starShopManager.GetCompleteMaxId());
		analyticsEvent.AddUserProperty("cohort", cohortAnalyticConverter.ConvertToCorrectValue(playerPaymentsStats.GetSumPriceAverage()));
		foreach (KeyValuePair<SimpleCurrency.CurrencyKey, SimpleCurrency> item in playerBasics.Balance.CurrenciesDict)
		{
			SimpleCurrency value = item.Value;
			if (value.CurrencyType != CurrencyType.LovePoints)
			{
				if (value.CurrencyType == CurrencyType.MiniEvent)
				{
					analyticsEvent.AddUserProperty($"{value.CurrencyType}{value.Identificator}", value.Count);
				}
				else
				{
					analyticsEvent.AddUserProperty(value.CurrencyType.ToString(), value.Count);
				}
			}
		}
	}
}
