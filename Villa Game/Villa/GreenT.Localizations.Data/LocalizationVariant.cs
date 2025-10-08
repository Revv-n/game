namespace GreenT.Localizations.Data;

public class LocalizationVariant
{
	public string Key { get; }

	public string Name { get; }

	public bool IsDefault { get; }

	public LocalizationVariant(string key, bool isDefault, string name)
	{
		Key = key;
		IsDefault = isDefault;
		Name = name;
	}
}
