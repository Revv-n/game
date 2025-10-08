using StripClub.Model;

namespace Merge.MotionDesign;

public sealed class TaskRewardsMainFactory
{
	private readonly TaskStarManager _taskStarManager;

	private readonly TaskJewelManager _taskJewelManager;

	private readonly TaskContractsManager _taskContractsManager;

	public TaskRewardsMainFactory(TaskStarManager taskStarManager, TaskJewelManager taskJewelManager, TaskContractsManager taskContractsManager)
	{
		_taskStarManager = taskStarManager;
		_taskJewelManager = taskJewelManager;
		_taskContractsManager = taskContractsManager;
	}

	public FlyingCurrency Create(CurrencyType currencyType)
	{
		FlyingCurrency result = null;
		switch (currencyType)
		{
		case CurrencyType.Star:
			result = _taskStarManager.GetView();
			break;
		case CurrencyType.Jewel:
			result = _taskJewelManager.GetView();
			break;
		case CurrencyType.Contracts:
			result = _taskContractsManager.GetView();
			break;
		}
		return result;
	}
}
