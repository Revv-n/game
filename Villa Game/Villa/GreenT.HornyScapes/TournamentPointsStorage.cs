using System;
using GreenT.Data;

namespace GreenT.HornyScapes;

[Serializable]
[MementoHolder]
public sealed class TournamentPointsStorage : ISavableState
{
	[Serializable]
	public class TournamentPointsMemento : Memento
	{
		public float AdditivePoints;

		public float LastPower;

		public TournamentPointsMemento(TournamentPointsStorage tournamentPointsStorage)
			: base(tournamentPointsStorage)
		{
			Save(tournamentPointsStorage);
		}

		public Memento Save(TournamentPointsStorage tournamentPointsStorage)
		{
			AdditivePoints = tournamentPointsStorage.AdditivePoints;
			LastPower = tournamentPointsStorage.LastPower;
			return this;
		}
	}

	private const string UNIQUE_KEY = "tournament_points.data";

	private string _uniqueKey;

	public float AdditivePoints;

	public float LastPower;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public TournamentPointsStorage()
	{
		_uniqueKey = "tournament_points.data";
	}

	public string UniqueKey()
	{
		return _uniqueKey;
	}

	public Memento SaveState()
	{
		return new TournamentPointsMemento(this);
	}

	public void LoadState(Memento memento)
	{
		TournamentPointsMemento tournamentPointsMemento = (TournamentPointsMemento)memento;
		AdditivePoints = tournamentPointsMemento.AdditivePoints;
		LastPower = tournamentPointsMemento.LastPower;
	}
}
