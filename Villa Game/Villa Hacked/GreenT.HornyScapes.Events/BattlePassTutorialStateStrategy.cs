using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class BattlePassTutorialStateStrategy : ITutorialStateStrategy
{
	private readonly GameObject _container;

	public BattlePassTutorialStateStrategy(GameObject container)
	{
		_container = container;
	}

	public void SetActive(bool isActive)
	{
		_container.SetActive(isActive);
	}
}
