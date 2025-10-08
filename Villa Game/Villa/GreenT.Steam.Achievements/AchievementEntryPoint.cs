using System;
using GreenT.HornyScapes;
using GreenT.Steam.Achievements.Callbacks;
using UniRx;

namespace GreenT.Steam.Achievements;

public class AchievementEntryPoint : IDisposable
{
	private readonly BindCallbackService _bindCallbackService;

	private readonly AchievementStats _achievementStats;

	private readonly GameStarter _gameStarter;

	private IDisposable _stream;

	public AchievementEntryPoint(BindCallbackService bindCallbackService, AchievementStats achievementStats, GameStarter gameStarter)
	{
		_bindCallbackService = bindCallbackService;
		_achievementStats = achievementStats;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_bindCallbackService.Bind();
		_stream = _gameStarter.IsGameActive.Where((bool x) => x).Take(1).Subscribe(delegate
		{
			Track();
		});
	}

	private void Track()
	{
		try
		{
			_achievementStats.Track();
		}
		catch (Exception value)
		{
			System.Console.WriteLine(value);
			throw;
		}
	}

	public void Dispose()
	{
		_stream?.Dispose();
	}
}
