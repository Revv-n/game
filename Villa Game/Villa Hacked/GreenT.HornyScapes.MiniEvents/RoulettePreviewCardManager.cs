using StripClub.Model;
using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public abstract class RoulettePreviewCardManager : CustomViewManager<LinkedContent, PromoCardView>
{
	protected RoulettePreviewCardFactory _customViewFactory;

	protected bool _isBig;

	[Inject]
	public void Init(RoulettePreviewCardFactory customViewFactory)
	{
		_customViewFactory = customViewFactory;
	}

	protected override PromoCardView Create(LinkedContent source)
	{
		return _customViewFactory.Create(source, _isBig);
	}

	protected override bool CheckAvailableView(PromoCardView view, LinkedContent source)
	{
		bool num = base.CheckAvailableView(view, source);
		bool flag = CheckSetCorrectContent(view, source);
		return num && flag;
	}

	protected abstract bool CheckSetCorrectContent(PromoCardView view, LinkedContent source);
}
