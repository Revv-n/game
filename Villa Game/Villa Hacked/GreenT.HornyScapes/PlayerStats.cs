using System;
using GreenT.Data;
using StripClub.Model.Shop;
using UniRx;

namespace GreenT.HornyScapes;

[MementoHolder]
public class PlayerStats : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[Serializable]
		public class BoughtInSectionData
		{
			public int SectionId;

			public int Value;
		}

		public int PressOnBuyButtonCount { get; private set; }

		public int PaymentCount { get; private set; }

		public Memento(PlayerStats stats)
			: base(stats)
		{
			PaymentCount = stats.PaymentCount.Value;
			PressOnBuyButtonCount = stats.CheckoutAttemptCount.Value;
		}

		private void SaveDict(PlayerStats stats)
		{
		}

		private void Update(BoughtInSection model, BoughtInSectionData data)
		{
			data.SectionId = model.SectionId;
			data.Value = model.Value.Value;
		}
	}

	public ReactiveDictionary<string, BoughtInSection> BoughtInSectionDict = new ReactiveDictionary<string, BoughtInSection>();

	public ReactiveProperty<int> PaymentCount { get; private set; } = new ReactiveProperty<int>(0);


	public ReactiveProperty<int> CheckoutAttemptCount { get; private set; } = new ReactiveProperty<int>(0);


	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void Initialize()
	{
		PaymentCount.Value = 0;
		CheckoutAttemptCount.Value = 0;
		BoughtInSectionDict.Clear();
	}

	public void AddBought(Lot lot)
	{
		string text = lot.TabID.ToString();
		BoughtInSection boughtInSection = default(BoughtInSection);
		if (BoughtInSectionDict.TryGetValue(text, ref boughtInSection))
		{
			ReactiveProperty<int> value = boughtInSection.Value;
			int value2 = value.Value;
			value.Value = value2 + 1;
		}
		else
		{
			BoughtInSectionDict.Add(text, new BoughtInSection(lot.TabID, 1));
		}
	}

	public void Dispose()
	{
		CheckoutAttemptCount?.Dispose();
		PaymentCount?.Dispose();
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		PaymentCount.Value = memento2.PaymentCount;
		CheckoutAttemptCount.Value = memento2.PressOnBuyButtonCount;
	}

	private void RestoreDict(Memento statsMemento)
	{
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public string UniqueKey()
	{
		return "player_stats";
	}
}
