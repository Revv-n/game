using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.Lootboxes.Data;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class LootboxStructureInitializerFactory : IFactory<ConfigParser.Folder, LootboxStructureInitializer>, IFactory
{
	private readonly IFactory<LootboxMapper, Lootbox> lootboxFactory;

	private readonly LootboxCollection lootboxCollection;

	public LootboxStructureInitializerFactory(IFactory<LootboxMapper, Lootbox> lootboxFactory, LootboxCollection lootboxCollection)
	{
		this.lootboxFactory = lootboxFactory;
		this.lootboxCollection = lootboxCollection;
	}

	public LootboxStructureInitializer Create(ConfigParser.Folder configStructure)
	{
		return new LootboxStructureInitializer(lootboxCollection, lootboxFactory);
	}
}
