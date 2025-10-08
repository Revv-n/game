using System;
using GreenT.HornyScapes.Data;

namespace GreenT.Localizations.Data;

[Serializable]
[Mapper]
public class LocalizationVariantMapper
{
	public string language_key;

	public bool is_default;

	public string localize_name;
}
