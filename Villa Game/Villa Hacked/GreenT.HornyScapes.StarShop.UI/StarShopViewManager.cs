using StripClub.UI;
using Zenject;

namespace GreenT.HornyScapes.StarShop.UI;

public class StarShopViewManager : ViewManagerBase<StarShopView>
{
	private IFactory<StarShopView> factory;

	[Inject]
	public void Init(IFactory<StarShopView> factory)
	{
		this.factory = factory;
	}

	protected override StarShopView Create()
	{
		return factory.Create();
	}
}
