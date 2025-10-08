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
		IObservable<bool> observable = minieventTabs.ToObservable().SelectMany((MiniEventActivityTab tab) => tab.IsAnyActionAvailable).Merge();
		IObservable<bool> source = miniEvent.IsSpawned.Merge(miniEvent.WasFirstTimeSeen).Merge(observable);
		_disposable = source.Subscribe(delegate
		{
			bool flag = !miniEvent.WasFirstTimeSeen.Value && miniEvent.IsSpawned.Value;
			_pimp.SetActive(minieventTabs.Any((MiniEventActivityTab tab) => tab.IsAnyActionAvailable.Value) || flag);
		});
	}
}
