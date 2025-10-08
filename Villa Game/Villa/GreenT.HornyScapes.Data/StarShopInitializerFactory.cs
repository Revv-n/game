using GreenT.HornyScapes.StarShop;
using GreenT.HornyScapes.StarShop.Data;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class StarShopInitializerFactory : IFactory<ConfigParser.Folder, StarShopStructureInitializer>, IFactory
{
	private readonly StarShopManager starShopManager;

	private readonly IFactory<StarShopMapper, StarShopItem> starShopFactory;

	public StarShopInitializerFactory(StarShopManager starShopManager, IFactory<StarShopMapper, StarShopItem> starShopFactory)
	{
		this.starShopManager = starShopManager;
		this.starShopFactory = starShopFactory;
	}

	public StarShopStructureInitializer Create(ConfigParser.Folder configStructure)
	{
		return new StarShopStructureInitializer(starShopManager, starShopFactory);
	}
}
