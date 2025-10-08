using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins;

public class SkinStructureInitializer : StructureInitializerViaArray<SkinMapper, Skin>
{
	public SkinStructureInitializer(IManager<Skin> manager, IFactory<SkinMapper, Skin> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
