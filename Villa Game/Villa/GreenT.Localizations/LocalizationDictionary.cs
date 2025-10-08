using System;

namespace GreenT.Localizations;

[Serializable]
public class LocalizationDictionary : SerializableDictionary<string, string>
{
	public bool IsLocal;
}
