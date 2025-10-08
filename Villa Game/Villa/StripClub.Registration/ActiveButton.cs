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
		checkStream = (from _checkers in checkers.Select((AbstractChecker _checker) => _checker.OnUpdate).CombineLatest().StartWith(checkers)
			select _checkers.All((AbstractChecker _checker) => _checker.State == AbstractChecker.ValidationState.IsValid)).TakeUntilDisable(this).ToReactiveCommand(initialValue: false).BindTo(Button)
			.AddTo(this);
	}
}
