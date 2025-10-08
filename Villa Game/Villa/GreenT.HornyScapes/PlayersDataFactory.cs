using GreenT.Data;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public class PlayersDataFactory : IFactory<PlayersData>, IFactory
{
	private readonly ISaver saver;

	private readonly Currencies balance;

	private readonly PlayerEnergyFactory energyFactory;

	private readonly PlayerEventEnergyFactory eventEnergyFactory;

	public PlayersDataFactory(ISaver saver, Currencies balance, PlayerEnergyFactory energyFactory, PlayerEventEnergyFactory eventEnergyFactory)
	{
		this.saver = saver;
		this.balance = balance;
		this.energyFactory = energyFactory;
		this.eventEnergyFactory = eventEnergyFactory;
	}

	public PlayersData Create()
	{
		PlayersData playersData = new PlayersData(balance, energyFactory, eventEnergyFactory);
		saver.Add(playersData);
		return playersData;
	}
}
