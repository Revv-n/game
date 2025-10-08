using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Registration;

[RequireComponent(typeof(Button))]
public class ActiveButton : MonoBehaviour
{
	[SerializeField]
	private AbstractChecker[] checkers;

	private IDisposable checkStream;

	[field: SerializeField]
	public Button Button { get; private set; }

	private void OnValidate()
	{
		Button = GetComponent<Button>();
	}

	public void Set(params AbstractChecker[] checkers)
	{
		this.checkers = checkers;
		TrackCheckers();
	}

	private void OnEnable()
	{
		TrackCheckers();
	}

	private void TrackCheckers()
	{
		checkStream?.Dispose();
		checkStream = DisposableExtensions.AddTo<IDisposable>(ReactiveCommandExtensions.BindTo((IReactiveCommand<Unit>)(object)ReactiveCommandExtensions.ToReactiveCommand(Observable.TakeUntilDisable<bool>(Observable.Select<IList<AbstractChecker>, bool>(Observable.StartWith<IList<AbstractChecker>>(Observable.CombineLatest<AbstractChecker>(checkers.Select((AbstractChecker _checker) => _checker.OnUpdate)), (IList<AbstractChecker>)checkers), (Func<IList<AbstractChecker>, bool>)((IList<AbstractChecker> _checkers) => _checkers.All((AbstractChecker _checker) => _checker.State == AbstractChecker.ValidationState.IsValid))), (Component)this), false), Button), (Component)this);
	}
}
