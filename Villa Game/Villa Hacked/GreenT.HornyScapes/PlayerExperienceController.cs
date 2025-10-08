using System;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

[Obsolete("Use BattlePasLevelInfoCase")]
public class PlayerExperienceController : IPlayerExpController
{
	private readonly PlayerExperience _playerExperience;

	public IReadOnlyReactiveProperty<int> Level => (IReadOnlyReactiveProperty<int>)(object)ReactivePropertyExtensions.ToReadOnlyReactiveProperty<int>((IObservable<int>)_playerExperience.Level);

	public IObservable<int> OnProgressUpdate => Observable.AsObservable<int>((IObservable<int>)_playerExperience.XP);

	public ReactiveProperty<int> Target { get; set; } = new ReactiveProperty<int>(1);


	public int XPCount => _playerExperience.XP.Value;

	[Inject]
	public PlayerExperienceController(PlayerExperience playerExperience)
	{
		_playerExperience = playerExperience;
	}

	public bool IsComplete()
	{
		return _playerExperience.XP.Value >= Target.Value;
	}
}
