using UnityEngine;

namespace GreenT.HornyScapes.Events;

public sealed class EventTutorialStateStrategy : ITutorialStateStrategy
{
	private readonly GameObject _container;

	public EventTutorialStateStrategy(GameObject container)
	{
		_container = container;
	}

	public void SetActive(bool isActive)
	{
		_container.SetActive(isActive);
	}
}
