using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Dates.Models;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Dates.Factories;

public class ComingSoonDateLinkedContentFactory : IFactory<int, LinkedContentAnalyticData, LinkedContent, ComingSoonDateLinkedContent>, IFactory
{
	public ComingSoonDateLinkedContent Create(int dateId, LinkedContentAnalyticData analyticData, LinkedContent nestedContent = null)
	{
		return new ComingSoonDateLinkedContent(dateId, analyticData, nestedContent);
	}
}
