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
		Assert.IsNotNull(current);
		UpdateState();
		IObservable<Unit> observable = lotManager.Collection.Select((Lot _lot) => from _isOpen in _lot.Locker.IsOpen.Skip(1)
			where _isOpen
			select _isOpen).Merge().AsUnitObservable();
		trackStream = (from _signal in signalBus.GetStream<ViewUpdateSignal>()
			where _signal.View is IView<Lot>
			select _signal).AsUnitObservable().Merge(signalBus.GetStream<LotBoughtSignal>().AsUnitObservable()).Merge(observable)
			.Subscribe(delegate
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
