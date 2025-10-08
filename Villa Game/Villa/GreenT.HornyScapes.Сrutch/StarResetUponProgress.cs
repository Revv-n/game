using System;
using GreenT.Data;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Сrutch;

[MementoHolder]
public sealed class StarResetUponProgress : ISavableState, IDisposable
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		[field: SerializeField]
		public bool isResetCompleted { get; private set; }

		public Memento(StarResetUponProgress task)
			: base(task)
		{
			isResetCompleted = task.isResetCompleted;
		}
	}

	private bool isResetCompleted;

	private readonly ISaver saver;

	private readonly ICurrencyProcessor currencyProcessor;

	private readonly IPlayerBasics playerBasics;

	private readonly IConstants<ILocker> lockerConstant;

	private const string switchToLockerID = "star_reset_upon_progress";

	private readonly CompositeDisposable disposable = new CompositeDisposable();

	private const string uniqueKey = "star_reset_upon_progress";

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public StarResetUponProgress(ISaver saver, ICurrencyProcessor currencyProcessor, IConstants<ILocker> lockerConstant)
	{
		this.saver = saver;
		this.currencyProcessor = currencyProcessor;
		this.lockerConstant = lockerConstant;
	}

	public void Initialize()
	{
		saver.Add(this);
		lockerConstant["star_reset_upon_progress"].IsOpen.Where((bool x) => x).Subscribe(Check).AddTo(disposable);
	}

	private void Check(bool isOpen)
	{
		if (!isResetCompleted && isOpen && currencyProcessor.TryReset(CurrencyType.Star))
		{
			isResetCompleted = true;
		}
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		isResetCompleted = memento2.isResetCompleted;
	}

	public string UniqueKey()
	{
		return "star_reset_upon_progress";
	}

	public void Dispose()
	{
		disposable?.Dispose();
	}
}
