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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		_miniEventTabsManager = miniEventTabsManager;
		_disposables = new CompositeDisposable();
	}

	public void Dispose()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Clear();
		}
		CompositeDisposable disposables2 = _disposables;
		if (disposables2 != null)
		{
			disposables2.Dispose();
		}
	}

	public void AdministrateMiniEvent(MiniEvent miniEvent)
	{
		IEnumerable<MiniEventActivityTab> activityTabs = _miniEventTabsManager.Collection.Where((MiniEventActivityTab tab) => tab.EventIdentificator[0] == miniEvent.EventId);
		if (activityTabs == null)
		{
			return;
		}
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.AsUnitObservable<bool>(Observable.Merge<bool>(Observable.SelectMany<MiniEventActivityTab, bool>(Observable.ToObservable<MiniEventActivityTab>(activityTabs), (Func<MiniEventActivityTab, IObservable<bool>>)((MiniEventActivityTab _tab) => (IObservable<bool>)_tab.IsAnyContentAvailable)), Array.Empty<IObservable<bool>>())), (Action<Unit>)delegate
		{
			miniEvent.IsAnyContentAvailable.Value = activityTabs.Any((MiniEventActivityTab tab) => tab.IsAnyContentAvailable.Value);
		}), (ICollection<IDisposable>)_disposables);
	}
}
