using System.Collections.Generic;
using GreenT.Localizations;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Cheats;

public class UniqueSymbolsGetter
{
	private LocalizationProvider provider;

	private HashSet<char> symbols;

	public UniqueSymbolsGetter(LocalizationProvider provider)
	{
		this.provider = provider;
		symbols = new HashSet<char>();
	}

	public HashSet<char> GetUniqueSymbols()
	{
		symbols.Clear();
		foreach (string localizationKey in provider.LocalizationKeys)
		{
			string text = provider.TryGetValue(localizationKey);
			for (int i = 0; i < text.Length; i++)
			{
				symbols.Add(text[i]);
			}
		}
		return symbols;
	}
}
