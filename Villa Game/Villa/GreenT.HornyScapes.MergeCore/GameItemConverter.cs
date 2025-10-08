using GreenT.HornyScapes.GameItems;
using Merge;
using Zenject;

namespace GreenT.HornyScapes.MergeCore;

public class GameItemConverter
{
	private readonly GameItemConfigManager _gameItemConfigManager;

	[Inject]
	public GameItemConverter(GameItemConfigManager gameItemConfigManager)
	{
		_gameItemConfigManager = gameItemConfigManager;
	}

	public GIConfig TryConvert(GIData data)
	{
		return _gameItemConfigManager.GetConfigOrNull(data.Key);
	}
}
