using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Presents.Analytics;
using GreenT.HornyScapes.Presents.Models;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Presents.Factories;

public class PresentContentFactory : IFactory<string, CurrencySelector, int, LinkedContentAnalyticData, LinkedContent, PresentLinkedContent>, IFactory, IFactory<string, int, LinkedContentAnalyticData, PresentLinkedContent>
{
	private readonly PresentsManager _presentsManager;

	private readonly PresentsAnalytic _presentsAnalytic;

	public PresentContentFactory(PresentsManager presentsManager, PresentsAnalytic presentsAnalytic)
	{
		_presentsManager = presentsManager;
		_presentsAnalytic = presentsAnalytic;
	}

	public PresentLinkedContent Create(string id, CurrencySelector selector, int quantity, LinkedContentAnalyticData analyticData, LinkedContent nested = null)
	{
		return new PresentLinkedContent(_presentsManager.Get(id), quantity, analyticData, _presentsAnalytic, selector.Identificator, nested);
	}

	public PresentLinkedContent Create(string id, int quantity, LinkedContentAnalyticData analyticData)
	{
		Selector selector = SelectorTools.CreateSelector(id);
		return Create(id, selector as CurrencySelector, quantity, analyticData);
	}
}
