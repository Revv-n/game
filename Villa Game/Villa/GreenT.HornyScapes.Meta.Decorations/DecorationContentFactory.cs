using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationContentFactory : IFactory<int, LinkedContentAnalyticData, LinkedContent, DecorationLinkedContent>, IFactory, IFactory<int, LinkedContentAnalyticData, DecorationLinkedContent>
{
	private readonly DecorationManager _decorationManager;

	private readonly RoomManager _house;

	private readonly DecorationController _decorationController;

	public DecorationContentFactory(DecorationManager decorationManager, RoomManager house, DecorationController decorationController)
	{
		_decorationManager = decorationManager;
		_house = house;
		_decorationController = decorationController;
	}

	public DecorationLinkedContent Create(int decorationId, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		Decoration item = _decorationManager.GetItem(decorationId);
		return new DecorationLinkedContent(decorationId, _house, _decorationController, item, analyticData, nestedContent);
	}

	public DecorationLinkedContent Create(int decorationId, LinkedContentAnalyticData analyticData)
	{
		return Create(decorationId, analyticData, null);
	}
}
