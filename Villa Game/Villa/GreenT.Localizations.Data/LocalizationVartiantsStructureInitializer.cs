using System.Collections.Generic;
using GreenT.HornyScapes.Data;
using Zenject;

namespace GreenT.Localizations.Data;

public class LocalizationVartiantsStructureInitializer : StructureInitializerViaArray<LocalizationVariantMapper, LocalizationVariant>
{
	public LocalizationVartiantsStructureInitializer(LocalizationVariantsProvider manager, LocalizationVariantsFactory factory, IEnumerable<IStructureInitializer> others = null)
		: base((IManager<LocalizationVariant>)manager, (IFactory<LocalizationVariantMapper, LocalizationVariant>)factory, others)
	{
	}
}
