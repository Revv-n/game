using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class RatingTutorialStateStrategy : ITutorialStateStrategy
{
	private readonly GameObject _container;

	public RatingTutorialStateStrategy(GameObject container)
	{
		_container = container;
	}

	public void SetActive(bool isActive)
	{
		_container.SetActive(isActive);
	}
}
