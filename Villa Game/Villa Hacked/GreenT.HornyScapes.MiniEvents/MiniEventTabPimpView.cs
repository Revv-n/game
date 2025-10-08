using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventTabPimpView : MonoView
{
	[SerializeField]
	private GameObject _pimp;

	private IDisposable _disposable;

	private MiniEventTabsManager _miniEventTabsManager;

	[Inject]
	private void Init(MiniEventTabsManager miniEventTabsManager)
	{
		_miniEventTabsManager = miniEventTabsManager;
	}

	private void OnDestroy()
	{
		_disposable?.Dispose();
	}

	public void StartTrack(MiniEvent miniEvent)
	{
		_disposable?.Dispose();
		IEnumerable<MiniEventActivityTab> minieventTabs = _miniEventTabsManager.Collection.Where((MiniEventActivityTab tab) => tab.EventIdentificator[0] == miniEvent.EventId);
		IObservable<bool> observable = Observable.Merge<bool>(Observable.SelectMany<MiniEventActivityTab, bool>(Observable.ToObservable<MiniEventActivityTab>(minieventTabs), (Func<MiniEventActivityTab, IObservable<bool>>)((MiniEventActivityTab tab) => (IObservable<bool>)tab.IsAnyActionAvailable)), Array.Empty<IObservable<bool>>());
		IObservable<bool> observable2 = Observable.Merge<bool>(Observable.Merge<bool>((IObservable<bool>)miniEvent.IsSpawned, new IObservable<bool>[1] { (IObservable<bool>)miniEvent.WasFirstTimeSeen }), new IObservable<bool>[1] { observable });
		_disposable = ObservableExtensions.Subscribe<bool>(observable2, (Action<bool>)delegate
		{
			bool flag = !miniEvent.WasFirstTimeSeen.Value && miniEvent.IsSpawned.Value;
			_pimp.SetActive(minieventTabs.Any((MiniEventActivityTab tab) => tab.IsAnyActionAvailable.Value) || flag);
		});
	}
}
