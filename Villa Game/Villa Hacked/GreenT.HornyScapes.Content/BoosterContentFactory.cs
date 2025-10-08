using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Booster;
using Zenject;

namespace GreenT.HornyScapes.Content;

public class BoosterContentFactory : IFactory<int, LinkedContentAnalyticData, BoosterLinkedContent>, IFactory
{
	private readonly GameSettings _gameSettings;

	private readonly BoosterService _boosterService;

	private readonly BoosterMapperManager _mapperManager;

	private BoosterContentFactory(GameSettings gameSettings, BoosterService boosterService, BoosterMapperManager mapperManager)
	{
		_gameSettings = gameSettings;
		_boosterService = boosterService;
		_mapperManager = mapperManager;
	}

	public BoosterLinkedContent Create(int id, LinkedContentAnalyticData analyticData)
	{
		BoosterMapper boosterMapper = _mapperManager.Collection.First((BoosterMapper item) => item.booster_id == id);
		return new BoosterLinkedContent(id, boosterMapper.bonus_type, _boosterService, _gameSettings, analyticData);
	}
}
