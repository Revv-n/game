using System;
using GreenT.Localizations;
using ModestTree;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.UI;

public class LocalizedTextMeshPro : MonoBehaviour
{
	[SerializeField]
	protected TMP_Text text;

	[SerializeField]
	protected string key;

	private object[] arguments = new object[0];

	private IDisposable localizationStream;

	private LocalizationService _localizationService;

	public string Key => key;

	public TMP_Text Text => text;

	[Inject]
	private void InnerInit(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	protected virtual void Awake()
	{
		Assert.IsNotNull((object)text);
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(key))
		{
			Init(key, arguments);
		}
	}

	public void Init(string key, params object[] arguments)
	{
		localizationStream?.Dispose();
		this.key = key;
		if (string.IsNullOrEmpty(key))
		{
			text.text = string.Empty;
		}
		else
		{
			SetArguments(arguments);
		}
	}

	public void SetArguments(params object[] arguments)
	{
		localizationStream?.Dispose();
		IObservable<string> observable = (IObservable<string>)_localizationService.ObservableText(key);
		if (arguments.Length != 0)
		{
			TryLocalizeArguments(arguments);
			observable = Observable.Select<string, string>(observable, (Func<string, string>)((string x) => string.Format(x, arguments)));
		}
		this.arguments = arguments;
		localizationStream = ObservableExtensions.Subscribe<string>(observable, (Action<string>)SetText);
	}

	protected virtual void SetText(string value)
	{
		text.text = value;
	}

	private void TryLocalizeArguments(object[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = _localizationService.Text(items[i].ToString());
		}
	}

	private void OnDisable()
	{
		localizationStream?.Dispose();
	}

	private void OnDestroy()
	{
		localizationStream?.Dispose();
	}
}
