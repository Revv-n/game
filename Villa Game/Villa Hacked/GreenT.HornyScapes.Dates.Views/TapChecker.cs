using System;
using System.Collections;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Dates.Views;

public sealed class TapChecker : MonoView
{
	[SerializeField]
	private Button _button;

	[SerializeField]
	private float _delay;

	private EyeView _eyeView;

	private bool _canClick;

	private WaitForSeconds _waiter;

	private Coroutine _coroutine;

	private IDisposable _clickingStream;

	private readonly Subject<Unit> _clicked = new Subject<Unit>();

	public IObservable<Unit> Clicked => Observable.AsObservable<Unit>((IObservable<Unit>)_clicked);

	[Inject]
	private void Init(EyeView eyeView)
	{
		_eyeView = eyeView;
	}

	private void Awake()
	{
		_waiter = new WaitForSeconds(_delay);
		_canClick = true;
	}

	private void OnEnable()
	{
		_button.onClick.AddListener(OnClicked);
		_clickingStream?.Dispose();
		_clickingStream = ObservableExtensions.Subscribe<bool>(_eyeView.Clicked, (Action<bool>)delegate(bool isEyeOpened)
		{
			OnEyeViewClicked(isEyeOpened);
		});
	}

	private void OnDisable()
	{
		_button.onClick.RemoveListener(OnClicked);
	}

	private void StopWaitClick()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
	}

	private IEnumerator WaitClick()
	{
		yield return _waiter;
		_canClick = true;
		_coroutine = null;
	}

	private void OnClicked()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!_canClick)
		{
			_eyeView.Reset();
			if (_coroutine == null)
			{
				_coroutine = StartCoroutine(WaitClick());
			}
		}
		else if (_eyeView.IsEyeClosed)
		{
			_clicked.OnNext(Unit.Default);
		}
	}

	private void OnEyeViewClicked(bool isEyeOpened)
	{
		_canClick = false;
		if (isEyeOpened)
		{
			StopWaitClick();
			_coroutine = StartCoroutine(WaitClick());
		}
	}
}
