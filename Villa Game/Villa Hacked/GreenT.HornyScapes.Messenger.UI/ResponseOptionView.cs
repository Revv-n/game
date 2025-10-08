using System;
using GreenT.HornyScapes.MergeCore;
using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class ResponseOptionView : Button, IView<ResponseOption>, IView
{
	internal class Factory : ViewFactory<ResponseOption, ResponseOptionView>
	{
		public Factory(DiContainer diContainer, Transform objectContainer, ResponseOptionView prefab)
			: base(diContainer, objectContainer, prefab)
		{
		}
	}

	internal class Manager : ViewManager<ResponseOption, ResponseOptionView>
	{
	}

	[SerializeField]
	private StatableComponent backgroundStates;

	[SerializeField]
	private TMP_Text optionText;

	protected Subject<ResponseOption> onSet = new Subject<ResponseOption>();

	private ItemView.Manager itemViewManager;

	private IMergeIconProvider _iconProvider;

	private LocalizationService _localizationService;

	private IDisposable _optionDisposable;

	public ResponseOption Source { get; private set; }

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

	public IObservable<ResponseOption> OnSet => Observable.AsObservable<ResponseOption>((IObservable<ResponseOption>)onSet);

	[Inject]
	private void Init(ItemView.Manager itemViewManager, IMergeIconProvider iconManager, LocalizationService localizationService)
	{
		this.itemViewManager = itemViewManager;
		_iconProvider = iconManager;
		_localizationService = localizationService;
	}

	public void Set(ResponseOption option)
	{
		Source = option;
		_optionDisposable?.Dispose();
		_optionDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localizationService.ObservableText(option.LocalizationKey), (Action<string>)delegate(string text)
		{
			optionText.text = text;
		});
		itemViewManager.HideAll();
		bool flag = true;
		foreach (IItemLot necessaryItem in option.NecessaryItems)
		{
			itemViewManager.GetView().Set(necessaryItem.Icon, necessaryItem.GetCurrentCount(), necessaryItem.TargetCount);
			flag = flag && necessaryItem.CheckIsEnough();
		}
		backgroundStates.Set(flag ? 1 : 0);
		onSet.OnNext(option);
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
		if (!isOn)
		{
			_optionDisposable?.Dispose();
		}
	}
}
