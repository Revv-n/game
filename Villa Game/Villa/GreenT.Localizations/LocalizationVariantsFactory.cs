using GreenT.Localizations.Data;
using Zenject;

namespace GreenT.Localizations;

public class LocalizationVariantsFactory : IFactory<LocalizationVariantMapper, LocalizationVariant>, IFactory
{
	public LocalizationVariant Create(LocalizationVariantMapper mapper)
	{
		return new LocalizationVariant(mapper.language_key, mapper.is_default, mapper.localize_name);
	}
}
