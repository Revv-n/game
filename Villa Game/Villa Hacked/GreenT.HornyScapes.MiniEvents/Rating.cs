using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model;

namespace GreenT.HornyScapes.MiniEvents;

[MementoHolder]
public sealed class Rating : ISavableState
{
	[Serializable]
	public class RatingMemento : Memento
	{
		public bool IsRewarded;

		public bool IsFinished;

		public RatingMemento(Rating rating)
			: base(rating)
		{
			Save(rating);
		}

		public Memento Save(Rating lot)
		{
			IsRewarded = lot.IsRewarded;
			IsFinished = lot.IsFinished;
			return this;
		}
	}

	private const string UNIQUE_KEY_PREFIX = "minievents.rating.";

	private string _uniqueKey;

	public int ID { get; private set; }

	public int Range { get; private set; }

	public bool IsRewarded { get; set; }

	public bool IsFinished { get; set; }

	public Dictionary<Range, IEnumerable<LinkedContent>> Rewards { get; private set; }

	public Dictionary<Range, LootboxLinkedContent> LootboxRewards { get; private set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public Rating(int id, int range, Dictionary<Range, IEnumerable<LinkedContent>> rewards, Dictionary<Range, LootboxLinkedContent> lootboxRewards)
	{
		ID = id;
		Range = range;
		Rewards = rewards;
		LootboxRewards = lootboxRewards;
		_uniqueKey = "minievents.rating." + ID;
	}

	public void Initialize()
	{
		IsRewarded = false;
		IsFinished = false;
	}

	public bool TryGetRewardForLevel(int level, out IEnumerable<LinkedContent> rewards)
	{
		return Rewards.TryGetValue(Rewards.Keys.FirstOrDefault((Range key) => level >= key.LowerBorder && level <= key.UpperBorder), out rewards);
	}

	public bool TryGetLootboxRewardForLevel(int level, out LootboxLinkedContent rewards)
	{
		return LootboxRewards.TryGetValue(LootboxRewards.Keys.FirstOrDefault((Range key) => level >= key.LowerBorder && level <= key.UpperBorder), out rewards);
	}

	public string UniqueKey()
	{
		return _uniqueKey;
	}

	public Memento SaveState()
	{
		return new RatingMemento(this);
	}

	public void LoadState(Memento memento)
	{
		RatingMemento ratingMemento = (RatingMemento)memento;
		IsRewarded = ratingMemento.IsRewarded;
		IsFinished = ratingMemento.IsFinished;
	}
}
