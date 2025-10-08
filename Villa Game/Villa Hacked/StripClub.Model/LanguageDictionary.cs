using System;
using GreenT.Localizations;

namespace StripClub.Model;

[Serializable]
public class LanguageDictionary : SerializableDictionary<string, LocalizationDictionary>
{
}
