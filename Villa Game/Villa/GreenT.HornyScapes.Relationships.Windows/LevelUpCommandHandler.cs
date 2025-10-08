using System;
using GreenT.HornyScapes.Relationships.Animations;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Relationships.Windows;

public class LevelUpCommandHandler : MonoView<Relationship>, IDisposable
{
	private LevelUpCommandStorage _levelUpCommandStorage;

	private LevelUpAnimationService _levelUpAnimationService;

	private IDisposable _trackStream;

	private readonly Subject<Unit> _levelUp = new Subject<Unit>();

	public IObservable<Unit> LevelUp => _levelUp.AsObservable();

	[Inject]
	private void Init(LevelUpCommandStorage levelUpCommandStorage, LevelUpAnimationService levelUpAnimationService)
	{
		_levelUpCommandStorage = levelUpCommandStorage;
		_levelUpAnimationService = levelUpAnimationService;
	}

	public override void Set(Relationship source)
	{
		base.Set(source);
		if (_levelUpCommandStorage.TryGet(source, out var command2))
		{
			command2.Execute(_levelUpAnimationService);
		}
		_trackStream = _levelUpCommandStorage.OnNew.Where((LevelUpCommand _) => _levelUpCommandStorage.TryGet(source, out var _)).Do(delegate
		{
			_levelUp.OnNext(Unit.Default);
		}).Subscribe(delegate(LevelUpCommand command)
		{
			command.Execute(_levelUpAnimationService);
		});
	}

	private void OnDisable()
	{
		Dispose();
	}

	public void Dispose()
	{
		_trackStream?.Dispose();
	}
}
