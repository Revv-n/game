using System;
using GreenT.Data;
using UnityEngine;

namespace GreenT.HornyScapes.Analytics;

[MementoHolder]
public class GameStartStats : ISavableState
{
	public enum State
	{
		FirstPlay,
		HasPlayed
	}

	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public State State { get; private set; }

		public Memento(GameStartStats gameStartStats)
			: base(gameStartStats)
		{
			State = gameStartStats.CurrentState;
		}
	}

	public State CurrentState;

	private const string GameStartStatsUniqueKey = "game.start.state";

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public string UniqueKey()
	{
		return "game.start.state";
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		CurrentState = memento2.State;
	}
}
