using System;
using GreenT.Net;
using GreenT.Net.User;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class LoadingIndicator : MonoBehaviour, IDisposable
{
	[SerializeField]
	private float _rotateSpeed = 45f;

	private AuthorizationRequestProcessor _authorizationRequestProcessor;

	private GameStarter _gameStarter;

	private IDisposable _gameReadyStream;

	private IDisposable _animationStream;

	[Inject]
	private void Init([InjectOptional] AuthorizationRequestProcessor authorizationRequestProcessor, GameStarter gameStarter)
	{
		_authorizationRequestProcessor = authorizationRequestProcessor;
		_gameStarter = gameStarter;
	}

	private void Start()
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			_authorizationRequestProcessor.AddListener(OnAuthResponse);
			_gameReadyStream = _gameStarter.IsGameReadyToStart.Where((bool state) => state).Subscribe(delegate
			{
				Hide();
			});
		}
		Hide();
	}

	private void OnDestroy()
	{
		Dispose();
	}

	private void OnAuthResponse(Response<UserDataMapper> response)
	{
		Show();
	}

	private void Show()
	{
		base.gameObject.SetActive(value: true);
		_animationStream = Observable.EveryUpdate().Subscribe(delegate
		{
			base.transform.Rotate(Vector3.back * _rotateSpeed * Time.deltaTime);
		});
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
		_animationStream?.Dispose();
	}

	public void Dispose()
	{
		_gameReadyStream?.Dispose();
		_authorizationRequestProcessor?.RemoveListener(OnAuthResponse);
	}
}
