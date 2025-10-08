using System;
using StripClub.Stories;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Dates.Views;

public sealed class EyeView : MonoView
{
	[SerializeField]
	private Button _eyeButton;

	[SerializeField]
	private Image _eyeIcon;

	[SerializeField]
	private Sprite _openedEye;

	[SerializeField]
	private Sprite _closedEye;

	[SerializeField]
	private OverView _overView;

	[SerializeField]
	private GameObject _phraseContainer;

	[SerializeField]
	private Image _fade;

	private bool _isEyeClosed;

	private readonly Subject<bool> _clicked = new Subject<bool>();

	public bool IsEyeClosed => _isEyeClosed;

	public IObservable<bool> Clicked => Observable.AsObservable<bool>((IObservable<bool>)_clicked);

	public void Reset()
	{
		_isEyeClosed = true;
		UpdateEye();
		_overView.SetActive(_isEyeClosed);
		_fade.enabled = _isEyeClosed;
		_phraseContainer.SetActive(_isEyeClosed);
	}

	private void OnEnable()
	{
		_eyeButton.onClick.AddListener(OnEyeClicked);
	}

	private void OnDisable()
	{
		_eyeButton.onClick.RemoveListener(OnEyeClicked);
	}

	private void UpdateEye()
	{
		_eyeIcon.sprite = (_isEyeClosed ? _openedEye : _closedEye);
	}

	private void OnEyeClicked()
	{
		_isEyeClosed = !_isEyeClosed;
		UpdateEye();
		_overView.SetActive(_isEyeClosed);
		_phraseContainer.SetActive(_isEyeClosed);
		_fade.enabled = _isEyeClosed;
		_clicked.OnNext(_isEyeClosed);
	}
}
