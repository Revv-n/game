using System;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes;

[Obsolete("Use BattlePasLevelInfoCase")]
public class PlayerExperienceController : IPlayerExpController
{
	private readonly PlayerExperience _playerExperience;

	public IReadOnlyReactiveProperty<int> Level => _playerExperience.Level.ToReadOnlyReactiveProperty();

	public IObservable<int> OnProgressUpdate => _playerExperience.XP.AsObservable();

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
