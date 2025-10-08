using System;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Loading.UI;

public class AnimateLoadProgressOnPreload : MonoBehaviour
{
	[Range(0f, 1f)]
	[SerializeField]
	private float targetPercent = 0.3f;

	[SerializeField]
	private float animationSeconds = 4f;

	private ILoadingScreen _loadingScreen;

	private GameStarter _gameStarter;

	private IDisposable _destroyStream;

	[Inject]
	public void Init(ILoadingScreen loadingScreen, GameStarter gameStarter)
	{
		_loadingScreen = loadingScreen;
		_gameStarter = gameStarter;
	}

	private void Start()
	{
		_loadingScreen.SetProgress(targetPercent, animationSeconds);
		_destroyStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameReadyToStart, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			UnityEngine.Object.Destroy(this);
		});
	}

	private void OnDisable()
	{
		_destroyStream?.Dispose();
	}
}
