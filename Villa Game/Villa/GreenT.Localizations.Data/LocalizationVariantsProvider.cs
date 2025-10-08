using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.Localizations.Data;

public class LocalizationVariantsProvider : SimpleManager<LocalizationVariant>
{
	public LocalizationVariant GetDefaultVariant()
	{
		return Collection.FirstOrDefault((LocalizationVariant variant) => variant.IsDefault);
	}

	public LocalizationVariant GetLocalizationVariant(string key)
	{
		return Collection.FirstOrDefault((LocalizationVariant variant) => variant.Key == key);
	}

	public bool TryGetLocalizationVariant(string key, out LocalizationVariant variant)
	{
		variant = GetLocalizationVariant(key);
		return variant != null;
	}
}
