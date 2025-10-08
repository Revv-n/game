using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventAdministrator : IDisposable
{
	private readonly MiniEventTabsManager _miniEventTabsManager;

	private CompositeDisposable _disposables;

	public MiniEventAdministrator(MiniEventTabsManager miniEventTabsManager)
	{
		_miniEventTabsManager = miniEventTabsManager;
		_disposables = new CompositeDisposable();
	}

	public void Dispose()
	{
		_disposables?.Clear();
		_disposables?.Dispose();
	}

	public void AdministrateMiniEvent(MiniEvent miniEvent)
	{
		IEnumerable<MiniEventActivityTab> activityTabs = _miniEventTabsManager.Collection.Where((MiniEventActivityTab tab) => tab.EventIdentificator[0] == miniEvent.EventId);
		if (activityTabs == null)
		{
			return;
		}
		activityTabs.ToObservable().SelectMany((MiniEventActivityTab _tab) => _tab.IsAnyContentAvailable).Merge()
			.AsUnitObservable()
			.Subscribe(delegate
			{
				miniEvent.IsAnyContentAvailable.Value = activityTabs.Any((MiniEventActivityTab tab) => tab.IsAnyContentAvailable.Value);
			})
			.AddTo(_disposables);
	}
}
