namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventShopRouletteSummonView : BaseShopRouletteSummonView<RouletteSummonLot>
{
	protected override void TryRedirect()
	{
		_lotRedirection.TryRedirect(OnSet.Value.SinglePrice.CompositeIdentificator);
	}
}
