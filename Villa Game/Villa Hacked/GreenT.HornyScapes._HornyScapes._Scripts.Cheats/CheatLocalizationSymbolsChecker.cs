using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes._HornyScapes._Scripts.Cheats;

public class CheatLocalizationSymbolsChecker : MonoBehaviour, IDisposable
{
	[SerializeField]
	private Button checkSymbols;

	[SerializeField]
	private Button changeFont;

	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private TMP_FontAsset[] fontAssets;

	private int currentFontIndex = -1;

	private UniqueSymbolsGetter uniqueSymbolsGetter;

	private StringBuilder builder;

	private Action getUniqueSymbolsAction;

	private Task getUniqueSymbolsTask;

	private IDisposable checkSymbolsDisposable;

	private IDisposable changeFontDisposable;

	[Inject]
	private void Construct(LocalizationProvider provider)
	{
		uniqueSymbolsGetter = new UniqueSymbolsGetter(provider);
		builder = new StringBuilder();
		getUniqueSymbolsAction = ShowAllSymbols;
		checkSymbolsDisposable = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(checkSymbols), (Action<Unit>)delegate
		{
			ShowAllSymbolsAsync();
		});
		changeFontDisposable = ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(changeFont), (Action<Unit>)delegate
		{
			ChangeFont();
		});
		ChangeFont();
	}

	private async void ShowAllSymbolsAsync()
	{
		Task task = getUniqueSymbolsTask;
		if (task == null || task.IsCompleted)
		{
			getUniqueSymbolsTask = Task.Run(getUniqueSymbolsAction);
			await getUniqueSymbolsTask;
			text.text = builder.ToString();
		}
	}

	private void ShowAllSymbols()
	{
		HashSet<char> uniqueSymbols = uniqueSymbolsGetter.GetUniqueSymbols();
		builder.Clear();
		foreach (char item in uniqueSymbols)
		{
			builder.Append(item);
			builder.Append(" ");
		}
	}

	private void ChangeFont()
	{
		currentFontIndex = (currentFontIndex + 1) % fontAssets.Length;
		text.font = fontAssets[currentFontIndex];
	}

	public void Dispose()
	{
		getUniqueSymbolsTask?.Dispose();
		checkSymbolsDisposable?.Dispose();
		changeFontDisposable?.Dispose();
	}
}
