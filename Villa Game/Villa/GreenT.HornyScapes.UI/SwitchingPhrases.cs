using System;
using System.Collections.Generic;
using GreenT.Localizations;
using GreenT.Types;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.UI;

public sealed class SwitchingPhrases : MonoBehaviour
{
	private const string loadingPhraseKey = "ui.loading_screen.phrases.{0}";

	private const string dots = "...";

	private const int pharsesRepeatProtectionCount = 3;

	private const double pharsesSwitchPeriodInSeconds = 1.0;

	private const double dotsUpdatePeriod = 0.2;

	private static readonly TimeSpan DotsUpdatePeriod = TimeSpan.FromSeconds(0.2);

	private TimeSpan phraseSwithcPeriod = TimeSpan.FromSeconds(1.0);

	[SerializeField]
	private TMP_Text progressText;

	[SerializeField]
	private TMP_Text dotsText;

	private IDisposable switchPhrasesStream;

	private IDisposable dotsAnimationStream;

	private IDisposable lozalizationInitialization;

	private LocalizationService _localizationService;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public void Start()
	{
		DisplayText(isActive: false);
	}

	public void StopAnimation()
	{
		lozalizationInitialization?.Dispose();
		switchPhrasesStream?.Dispose();
		dotsAnimationStream?.Dispose();
		DisplayText(isActive: false);
	}

	private void DisplayText(bool isActive)
	{
		progressText.gameObject.SetActive(isActive);
		dotsText.gameObject.SetActive(isActive);
	}

	public void StartAnimation()
	{
		DisplayText(isActive: true);
		RepeatProtectionInfiniteEnumerator<Func<string>> phraseEnumerator = new RepeatProtectionInfiniteEnumerator<Func<string>>(GetPhrasesList(), 3);
		StartSwitchPhrases(phraseEnumerator);
		StartAnimateDots();
	}

	private IList<Func<string>> GetPhrasesList()
	{
		List<Func<string>> list = new List<Func<string>>();
		int num = 1;
		string text = $"ui.loading_screen.phrases.{num}";
		string value = _localizationService.Text(text);
		while (!text.Equals(value))
		{
			string funcKey = text;
			list.Add(GetPhraseFunc);
			num++;
			text = $"ui.loading_screen.phrases.{num}";
			value = _localizationService.Text(text);
			string GetPhraseFunc()
			{
				return _localizationService.Text(funcKey);
			}
		}
		return list;
	}

	private void StartSwitchPhrases(IEnumerator<Func<string>> phraseEnumerator)
	{
		switchPhrasesStream?.Dispose();
		switchPhrasesStream = (from _ in Observable.Timer(TimeSpan.Zero, phraseSwithcPeriod, Scheduler.MainThreadIgnoreTimeScale).DoOnSubscribe(phraseEnumerator.Reset).TakeWhile((long _) => phraseEnumerator.MoveNext())
			select phraseEnumerator.Current).Subscribe(delegate(Func<string> _phrase)
		{
			progressText.text = _phrase();
		});
	}

	private void StartAnimateDots()
	{
		int dotsCount = "...".Length;
		IObservable<int> source = from i in Observable.Timer(TimeSpan.Zero, DotsUpdatePeriod, Scheduler.MainThreadIgnoreTimeScale)
			select (int)i % dotsCount;
		dotsAnimationStream?.Dispose();
		dotsAnimationStream = source.Select(GetCertainNumberOfDots).Subscribe(delegate(string _dots)
		{
			dotsText.text = _dots;
		});
	}

	private string GetCertainNumberOfDots(int dotsNum)
	{
		return "...".Substring(0, dotsNum);
	}
}
