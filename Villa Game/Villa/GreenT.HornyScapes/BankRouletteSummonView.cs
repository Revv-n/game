namespace GreenT.HornyScapes;

public sealed class BankRouletteSummonView : BaseShopRouletteSummonView<RouletteBankSummonLot>
{
	protected override void TryRedirect()
	{
		int goToBankTabId = OnSet.Value.GoToBankTabId;
		if (goToBankTabId != 0)
		{
			_lotRedirection.TryStraightRedirect(goToBankTabId);
		}
	}
}
