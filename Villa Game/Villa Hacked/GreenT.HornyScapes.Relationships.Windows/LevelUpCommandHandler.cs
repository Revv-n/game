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

	public IObservable<Unit> LevelUp => Observable.AsObservable<Unit>((IObservable<Unit>)_levelUp);

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
		_trackStream = ObservableExtensions.Subscribe<LevelUpCommand>(Observable.Do<LevelUpCommand>(Observable.Where<LevelUpCommand>(_levelUpCommandStorage.OnNew, (Func<LevelUpCommand, bool>)((LevelUpCommand _) => _levelUpCommandStorage.TryGet(source, out var _))), (Action<LevelUpCommand>)delegate
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			_levelUp.OnNext(Unit.Default);
		}), (Action<LevelUpCommand>)delegate(LevelUpCommand command)
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
