using System;
using GreenT.HornyScapes.StarShop.SubSystems;
using GreenT.HornyScapes.UI;
using Merge.Meta.RoomObjects;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.StarShop;

public class StarShopController : IDisposable
{
	private readonly StarShopSubscriptions subscriptions;

	private readonly StarShopBoardFlow boardFlow;

	private readonly StarShopGirlFlow girlFlow;

	private readonly StartFlow startFlow;

	private readonly ICurrencyProcessor currencyProcessor;

	public StarShopController(StarShopSubscriptions subscriptions, StarShopBoardFlow boardFlow, StarShopGirlFlow girlFlow, StartFlow startFlow, ICurrencyProcessor currencyProcessor)
	{
		this.subscriptions = subscriptions;
		this.boardFlow = boardFlow;
		this.girlFlow = girlFlow;
		this.startFlow = startFlow;
		this.currencyProcessor = currencyProcessor;
	}

	public void Initialize(bool isGameActive)
	{
		boardFlow.Activate(isGameActive);
		girlFlow.Activate(isGameActive);
		subscriptions.Activate(isGameActive);
	}

	public bool TrySetRewarded(IStarShopItem item)
	{
		bool flag = currencyProcessor.IsEnough(item.Cost);
		if (!flag)
		{
			return false;
		}
		currencyProcessor.TrySpent(item.Cost);
		startFlow.Launch(item);
		item.SetState(EntityStatus.Rewarded);
		return flag;
	}

	public void Dispose()
	{
		subscriptions.Dispose();
	}
}
