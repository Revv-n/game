using System;
using GreenT.Bonus;
using StripClub.UI;

namespace GreenT.HornyScapes.Booster;

[Serializable]
public class BoosterModel
{
	[Serializable]
	public class Memento
	{
		public int SequenceID { get; }

		public int ID { get; }

		public TimeSpan TimeLeft { get; }

		public Memento(BoosterModel model)
		{
			ID = model.ID;
			SequenceID = model._sequenceID;
			TimeLeft = model.Timer.TimeLeft;
		}
	}

	public readonly GenericTimer Timer = new GenericTimer();

	private readonly int _sequenceID;

	private readonly long _initTime;

	public int ID { get; }

	public ISimpleBonus Bonus { get; private set; }

	public BoosterModel(int id, int sequenceID, long initTime)
	{
		ID = id;
		_sequenceID = sequenceID;
		_initTime = initTime;
	}

	public void Activate(ISimpleBonus bonus, long time = 0L)
	{
		Bonus = bonus;
		long num = ((time == 0L) ? _initTime : time);
		Timer.Start(TimeSpan.FromSeconds(num));
		Bonus.Apply();
	}

	public void Prolong(long time)
	{
		Timer.TimeLeft += TimeSpan.FromSeconds(time);
	}

	public Memento SaveState()
	{
		return new Memento(this);
	}
}
