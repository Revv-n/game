using Zenject;

namespace GreenT.HornyScapes.Booster;

public class BoosterModelFactory : IFactory<int, BoosterMapper, BoosterModel>, IFactory
{
	private readonly DiContainer _container;

	public BoosterModelFactory(DiContainer container)
	{
		_container = container;
	}

	public BoosterModel Create(int sequenceID, BoosterMapper mapper)
	{
		object[] extraArgs = new object[3] { mapper.booster_id, sequenceID, mapper.booster_time };
		return _container.Instantiate<BoosterModel>(extraArgs);
	}
}
