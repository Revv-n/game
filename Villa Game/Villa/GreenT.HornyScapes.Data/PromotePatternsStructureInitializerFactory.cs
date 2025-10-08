using System.Collections.Generic;
using GreenT.HornyScapes.Card.Data;
using StripClub.Model.Cards;
using StripClub.Model.Data;
using Zenject;

namespace GreenT.HornyScapes.Data;

public class PromotePatternsStructureInitializerFactory : IFactory<ConfigParser.Folder, PromotePatternsStructureInitializer>, IFactory
{
	private readonly IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory;

	private readonly PromotePatterns promotePatterns;

	public PromotePatternsStructureInitializerFactory(IFactory<IEnumerable<PromotePatternMapper>, PromotePatterns> patternsFactory, PromotePatterns promotePatterns)
	{
		this.patternsFactory = patternsFactory;
		this.promotePatterns = promotePatterns;
	}

	public PromotePatternsStructureInitializer Create(ConfigParser.Folder configStructure)
	{
		return new PromotePatternsStructureInitializer(patternsFactory, promotePatterns);
	}
}
