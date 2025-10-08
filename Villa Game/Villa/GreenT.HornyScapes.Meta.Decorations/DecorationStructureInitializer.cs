using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationStructureInitializer : StructureInitializerViaArray<DecorationMapper, Decoration>
{
	public DecorationStructureInitializer(IManager<Decoration> manager, IFactory<DecorationMapper, Decoration> factory, IEnumerable<IStructureInitializer> others = null)
		: base(manager, factory, others)
	{
	}
}
