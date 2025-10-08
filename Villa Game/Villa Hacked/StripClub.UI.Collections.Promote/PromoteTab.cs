using System;
using System.Collections.Generic;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class PromoteTab : MonoBehaviour, IView
{
	public class Manager : ViewManager<PromoteTab>
	{
	}

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private GameObject noveltyIndicator;

	[SerializeField]
	private StatableComponent tabTextColorStates;

	[SerializeField]
	private ToggleWithLocker toggleWithLocker;

	[SerializeField]
	private Color lockColorText;

	private LocalizationService _localizationService;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	protected Subject<int> onActivate = new Subject<int>();

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	public IObservable<int> OnActivate => Observable.AsObservable<int>((IObservable<int>)onActivate);

	public int ID { get; private set; }

	public bool IsOn { get; private set; }

	public Toggle Toggle => toggleWithLocker;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
		toggleWithLocker.isOn = false;
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	private void Start()
	{
		tabTextColorStates.Set(IsOn ? 1 : 0);
	}

	public void Init(string titleKey, int settingsID)
	{
		ID = settingsID;
		_localizationDisposables.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(titleKey), (Action<string>)delegate(string text)
		{
			title.text = text;
		}), (ICollection<IDisposable>)_localizationDisposables);
	}

	public void SetLock(bool locked)
	{
		toggleWithLocker.SetLocker(locked);
		tabTextColorStates.Set(locked ? 2 : (IsOn ? 1 : 0));
	}

	public void OnValueChanged(bool isOn)
	{
		IsOn = isOn;
		UpdateView(isOn);
		if (isOn)
		{
			onActivate.OnNext(ID);
		}
	}

	public void UpdateView(bool isOn)
	{
		tabTextColorStates.Set(isOn ? 1 : 0);
	}

	public void ActivateIndicator(bool active)
	{
		noveltyIndicator.SetActive(active);
	}

	protected virtual void OnDestroy()
	{
		onActivate.OnCompleted();
		onActivate.Dispose();
		_localizationDisposables.Dispose();
	}
}
