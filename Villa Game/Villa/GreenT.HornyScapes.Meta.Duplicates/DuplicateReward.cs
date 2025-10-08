using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes.Meta.Duplicates;

public class DuplicateReward
{
	private readonly string[] _rewId;

	private readonly RewType[] _rewType;

	private readonly Selector[] _selector;

	private readonly int[] _quantity;

	private readonly LinkedContentFactory _contentFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	public int DuplicateID { get; }

	public DuplicateReward(int duplicateID, string[] rewId, RewType[] rewType, Selector[] selector, int[] quantity, LinkedContentFactory contentFactory, LinkedContentAnalyticDataFactory analyticDataFactory)
	{
		DuplicateID = duplicateID;
		_rewId = rewId;
		_rewType = rewType;
		_selector = selector;
		_quantity = quantity;
		_contentFactory = contentFactory;
		_analyticDataFactory = analyticDataFactory;
	}

	public LinkedContent GetReward()
	{
		LinkedContent linkedContent = null;
		for (int i = 0; i < _rewId.Length; i++)
		{
			Selector selector = _selector[i];
			LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(CurrencyAmplitudeAnalytic.SourceType.Duplicate);
			LinkedContent linkedContent2 = _contentFactory.Create(_rewType[i], selector, _quantity[i], 0, ContentType.Main, analyticData);
			if (linkedContent == null)
			{
				linkedContent = linkedContent2;
			}
			else
			{
				linkedContent.Insert(linkedContent2);
			}
		}
		return linkedContent;
	}
}
