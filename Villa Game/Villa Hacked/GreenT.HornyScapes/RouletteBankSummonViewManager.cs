using StripClub.UI;
using StripClub.UI.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public class RouletteBankSummonViewManager : CustomViewManager<RouletteBankSummonLot, BankRouletteSummonView>
{
	[Inject]
	private void Init(RouletteBankLotViewFactory viewFactory)
	{
		base.viewFactory = viewFactory;
	}

	protected override bool CheckAvailableView(BankRouletteSummonView view, RouletteBankSummonLot source)
	{
		if (base.CheckAvailableView(view, source))
		{
			return view.Source == source;
		}
		return false;
	}
}
