using System;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Exit;

public class ExitPushController : IInitializable, IDisposable
{
	private readonly ExitPopupOpener _exitPopupOpener;

	private readonly GameStarter _gameStarter;

	private IDisposable _stream;

	public ExitPushController(ExitPopupOpener exitPopupOpener, GameStarter gameStarter)
	{
		_exitPopupOpener = exitPopupOpener;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		PlayerWantsToQuit.AllowQuitting = true;
		_stream = _gameStarter.IsGameActive.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			PlayerWantsToQuit.AllowQuitting = false;
		});
		PlayerWantsToQuit.OnWantsToQuit = (Action<bool>)Delegate.Combine(PlayerWantsToQuit.OnWantsToQuit, new Action<bool>(ShowPopup));
	}

	private void ShowPopup(bool value)
	{
		_exitPopupOpener.OpenAdditive();
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
