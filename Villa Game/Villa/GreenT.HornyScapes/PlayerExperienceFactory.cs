using Zenject;

namespace GreenT.HornyScapes;

public class PlayerExperienceFactory : IFactory<PlayerExperience>, IFactory
{
	private readonly DiContainer _container;

	public PlayerExperienceFactory(DiContainer container)
	{
		_container = container;
	}

	public PlayerExperience Create()
	{
		return _container.Instantiate<PlayerExperience>();
	}
}
