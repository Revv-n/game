using System;

namespace StripClub.Model.Data;

[Serializable]
public class SimpleCurrencyDictionary : SerializableDictionary<SimpleCurrency.CurrencyKey, SimpleCurrency>
{
}
