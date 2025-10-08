using System;
using GreenT.Localizations;
using StripClub.Model;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

public class ResponseOptionView : Button, IView<ResponseOption>, IView
{
	internal class Manager : ViewManager<ResponseOption, ResponseOptionView>
	{
	}

	[SerializeField]
	private StatableComponent backgroundStates;

	[SerializeField]
	private TMP_Text optionText;

	private ItemView.Manager itemViewManager;

	private IPlayerItems playersItems;

	private LocalizationService _localizationService;

	private IDisposable _localizationDisposable;

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

	[Inject]
	private void Init(IPlayerItems playersItems, ItemView.Manager itemViewManager, LocalizationService localizationService)
	{
		this.playersItems = playersItems;
		this.itemViewManager = itemViewManager;
		_localizationService = localizationService;
	}

	public void Set(ResponseOption option)
	{
		Source = option;
		_localizationDisposable?.Dispose();
		_localizationDisposable = _localizationService.ObservableText(option.LocalizationKey).Subscribe(delegate(string text)
		{
			optionText.text = text;
		});
		itemViewManager.HideAll();
		bool flag = true;
		foreach (ItemLot necessaryItem in option.NecessaryItems)
		{
			ItemView view = itemViewManager.GetView();
			int playersItemCount = GetPlayersItemCount(necessaryItem);
			view.Set(necessaryItem.info.Icon, Mathf.Min(playersItemCount, necessaryItem.count), necessaryItem.count);
			flag = flag && playersItemCount >= necessaryItem.count;
		}
		backgroundStates.Set(flag ? 1 : 0);
	}

	private int GetPlayersItemCount(ItemLot necessaryItem)
	{
		int result = 0;
		if (playersItems.Contains(necessaryItem.info.Guid))
		{
			result = playersItems.GetItem(necessaryItem.info.Guid).Amount.Value;
		}
		return result;
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
		if (!isOn)
		{
			_localizationDisposable?.Dispose();
		}
	}
}
