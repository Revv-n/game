using System;
using GreenT.Data;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.BattlePassSpace;

[MementoHolder]
public class BattlePassStartData : ISavableState, IDisposable
{
	[Serializable]
	private class BattlePassStartMemento : Memento
	{
		public bool StartWindowShown;

		public bool IsTutorialEnd;

		public bool WasFirstTimeStarted;

		public bool WasFirstTimePushed;

		public bool WasPurchasedPremium;

		public bool IsPurchaseComplete;

		public bool IsPurchaseStarted;

		public BattlePassStartMemento(BattlePassStartData data)
			: base(data)
		{
			StartWindowShown = data.StartWindowShown;
			IsTutorialEnd = data.IsTutorialEnd;
			WasFirstTimeStarted = data.WasFirstTimeStarted.Value;
			WasFirstTimePushed = data.WasFirstTimePushed.Value;
			WasPurchasedPremium = data.PremiumPurchasedProperty.Value;
			IsPurchaseComplete = data.IsPurchaseComplete;
			IsPurchaseStarted = data.IsPurchaseStarted;
		}
	}

	private readonly ISaver saver;

	private readonly string saveKey;

	private readonly ReactiveProperty<bool> _wasFirstTimeStarted = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> _wasFirstTimePushed = new ReactiveProperty<bool>();

	private readonly ReactiveProperty<bool> _firstStartedProgress = new ReactiveProperty<bool>(true);

	private readonly ReactiveProperty<bool> _premiumPurchasedProperty = new ReactiveProperty<bool>();

	private IDisposable _premiumUpdateStream;

	private readonly ILocker _premiumLocker;

	public bool IsTutorialEnd { get; private set; }

	public bool StartWindowShown { get; private set; }

	public bool IsPurchaseComplete { get; private set; }

	public bool IsPurchaseStarted { get; private set; }

	public IReadOnlyReactiveProperty<bool> WasFirstTimeStarted => (IReadOnlyReactiveProperty<bool>)(object)_wasFirstTimeStarted;

	public IReadOnlyReactiveProperty<bool> WasFirstTimePushed => (IReadOnlyReactiveProperty<bool>)(object)_wasFirstTimePushed;

	public IReadOnlyReactiveProperty<bool> FirstStartedProgress => (IReadOnlyReactiveProperty<bool>)(object)_firstStartedProgress;

	public IReadOnlyReactiveProperty<bool> PremiumPurchasedProperty => (IReadOnlyReactiveProperty<bool>)(object)_premiumPurchasedProperty;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public BattlePassStartData(ISaver saver, string saveKey, ILocker premiumLocker)
	{
		this.saver = saver;
		this.saveKey = saveKey;
		_premiumLocker = premiumLocker;
		saver.Add(this);
	}

	public void Initialize(ReactiveProperty<bool> premiumPurchasedProperty)
	{
		if (!_premiumPurchasedProperty.Value)
		{
			_premiumUpdateStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)_premiumPurchasedProperty, (Action<bool>)delegate(bool value)
			{
				premiumPurchasedProperty.Value = value;
			});
		}
		else
		{
			premiumPurchasedProperty.Value = true;
		}
	}

	public void SetFirstTimeStarted()
	{
		_wasFirstTimeStarted.Value = true;
	}

	public void SetFirstTimePushed()
	{
		_wasFirstTimePushed.Value = true;
	}

	public void SetPurchaseStarted(bool value)
	{
		IsPurchaseStarted = value;
	}

	public void SetPremiumPurchased(bool value)
	{
		_premiumPurchasedProperty.Value = value;
	}

	public void SetStartedBattlePassProgress()
	{
		StartWindowShown = true;
		IsTutorialEnd = true;
		SetIsNotFirstStartedProgress();
	}

	public void SetIsNotFirstStartedProgress()
	{
		if (_firstStartedProgress.Value)
		{
			_firstStartedProgress.Value = false;
		}
	}

	public void SetPurchaseComplete(bool value)
	{
		IsPurchaseComplete = value;
	}

	public void Dispose()
	{
		_premiumUpdateStream?.Dispose();
	}

	public void Delete()
	{
		saver.Delete(this);
	}

	public string UniqueKey()
	{
		return saveKey;
	}

	public Memento SaveState()
	{
		return new BattlePassStartMemento(this);
	}

	public void LoadState(Memento memento)
	{
		BattlePassStartMemento battlePassStartMemento = (BattlePassStartMemento)memento;
		StartWindowShown = battlePassStartMemento.StartWindowShown;
		IsTutorialEnd = battlePassStartMemento.IsTutorialEnd;
		IsPurchaseComplete = battlePassStartMemento.IsPurchaseComplete;
		IsPurchaseStarted = battlePassStartMemento.IsPurchaseStarted;
		_wasFirstTimeStarted.Value = battlePassStartMemento.WasFirstTimeStarted;
		_wasFirstTimePushed.Value = battlePassStartMemento.WasFirstTimePushed;
		_premiumPurchasedProperty.Value = (_premiumLocker.IsOpen.Value ? _premiumLocker.IsOpen.Value : battlePassStartMemento.WasPurchasedPremium);
	}
}
