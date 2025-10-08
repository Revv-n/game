using System;
using GreenT.Data;

namespace GreenT.HornyScapes;

[Serializable]
[MementoHolder]
public class RatingData : ISavableState
{
	[Serializable]
	public class RatingDataMemento : Memento
	{
		public bool IsRewarded;

		public bool IsFinished;

		public string AuthorizationToken;

		public bool IsCheating;

		public float PlayerPower;

		public int EventID;

		public string RewardId;

		public int Place;

		public RatingDataMemento(RatingData ratingData)
			: base(ratingData)
		{
			Save(ratingData);
		}

		public Memento Save(RatingData ratingData)
		{
			IsRewarded = ratingData.IsRewarded;
			IsFinished = ratingData.IsFinished;
			AuthorizationToken = ratingData.AuthorizationToken;
			IsCheating = ratingData.IsCheating;
			PlayerPower = ratingData.PlayerPower;
			EventID = ratingData.EventID;
			Place = ratingData.Place;
			RewardId = ratingData.RewardId;
			return this;
		}
	}

	private const string UNIQUE_KEY_PREFIX = "rating.data_";

	private string _uniqueKey;

	public bool IsCheating;

	public bool IsRewarded;

	public bool IsFinished;

	public bool IsGlobal;

	public string AuthorizationToken;

	public float PlayerPower;

	public int Place;

	public Rating TargetRating { get; private set; }

	public int EventID { get; private set; }

	public int CalendarID { get; private set; }

	public string RewardId { get; set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public RatingData(int eventId, int calendarId, bool isGlobal, Rating rating)
	{
		EventID = eventId;
		CalendarID = calendarId;
		TargetRating = rating;
		IsGlobal = isGlobal;
		_uniqueKey = string.Format("{0}{1}_{2}_{3}", "rating.data_", EventID, CalendarID, TargetRating.ID);
	}

	public void Initialize()
	{
	}

	public string UniqueKey()
	{
		return _uniqueKey;
	}

	public Memento SaveState()
	{
		return new RatingDataMemento(this);
	}

	public void LoadState(Memento memento)
	{
		RatingDataMemento ratingDataMemento = (RatingDataMemento)memento;
		IsRewarded = ratingDataMemento.IsRewarded;
		IsFinished = ratingDataMemento.IsFinished;
		AuthorizationToken = ratingDataMemento.AuthorizationToken;
		IsCheating = ratingDataMemento.IsCheating;
		PlayerPower = ratingDataMemento.PlayerPower;
		EventID = ratingDataMemento.EventID;
		RewardId = ratingDataMemento.RewardId;
		Place = ratingDataMemento.Place;
	}
}
