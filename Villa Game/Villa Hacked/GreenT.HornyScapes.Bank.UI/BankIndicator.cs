using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using ModestTree;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BankIndicator : MonoBehaviour, IContentSelector, ISelector<ContentType>
{
	[SerializeField]
	private GameObject target;

	private LotManager lotManager;

	private SignalBus signalBus;

	private IDictionary<ContentType, BankTab.Manager> tabsCluster;

	private BankTab.Manager current;

	private IDisposable trackStream;

	[Inject]
	public void Init(LotManager lotManager, SignalBus signalBus, IDictionary<ContentType, BankTab.Manager> tabsCluster)
	{
		this.lotManager = lotManager;
		this.signalBus = signalBus;
		this.tabsCluster = tabsCluster;
	}

	private void Start()
	{
		Assert.IsNotNull((object)current);
		UpdateState();
		IObservable<Unit> observable = Observable.AsUnitObservable<bool>(Observable.Merge<bool>(lotManager.Collection.Select((Lot _lot) => Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)_lot.Locker.IsOpen, 1), (Func<bool, bool>)((bool _isOpen) => _isOpen)))));
		trackStream = ObservableExtensions.Subscribe<Unit>(Observable.Merge<Unit>(Observable.Merge<Unit>(Observable.AsUnitObservable<ViewUpdateSignal>(Observable.Where<ViewUpdateSignal>(signalBus.GetStream<ViewUpdateSignal>(), (Func<ViewUpdateSignal, bool>)((ViewUpdateSignal _signal) => _signal.View is IView<Lot>))), new IObservable<Unit>[1] { Observable.AsUnitObservable<LotBoughtSignal>(signalBus.GetStream<LotBoughtSignal>()) }), new IObservable<Unit>[1] { observable }), (Action<Unit>)delegate
		{
			UpdateState();
		});
	}

	public void Select(ContentType selector)
	{
		current = tabsCluster[selector];
		UpdateState();
	}

	private void UpdateState()
	{
		bool active = lotManager.Collection.Any((Lot _lot) => _lot.IsAvailable() && (!_lot.IsViewed || _lot.IsFree) && current.Collection.Any((BankTab _section) => _section.ID == _lot.TabID));
		target.SetActive(active);
	}

	private void OnDestroy()
	{
		trackStream?.Dispose();
	}
}
