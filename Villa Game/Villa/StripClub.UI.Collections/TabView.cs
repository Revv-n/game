using System;
using GreenT.Data;
using GreenT.Localizations;
using ModestTree;
using StripClub.Model.Cards;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Collections;

[MementoHolder]
public class TabView : MonoBehaviour, IView, ISavableState
{
	[Serializable]
	public class TabViewMemento : Memento
	{
		public bool hintActivity;

		public TabViewMemento(bool hintActivity, TabView tabView)
			: base(tabView)
		{
			this.hintActivity = hintActivity;
		}
	}

	public class Manager : ViewManager<TabView>
	{
	}

	private const string nameKey = "content.collections.tab.{0}.name";

	private const string progressFormat = "ui.collections.tab.progress";

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TextMeshProUGUI title;

	[SerializeField]
	private TextMeshProUGUI progressCount;

	[SerializeField]
	private GameObject hint;

	private CardsCollection cards;

	private CardsDisplayGrid grid;

	private LocalizationService _localizationService;

	private IDisposable hintStream;

	private string uniqueKey;

	public Tab Source { get; private set; }

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

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeInHierarchy;
	}

	[Inject]
	public void Init(CardsCollection cards, CardsDisplayGrid grid, LocalizationService localizationService)
	{
		Assert.IsNotNull(cards);
		Assert.IsNotNull(grid);
		this.cards = cards;
		this.grid = grid;
		_localizationService = localizationService;
		uniqueKey = "MenuTab_" + Source.ID;
	}

	public void Init(Tab tab)
	{
		Source = tab;
		string key = $"content.collections.tab.{tab.ID}.name";
		title.text = _localizationService.Text(key);
		int unlockedCards = cards.UnlockedCount(tab.ID);
		int totalCards = cards.Count(tab.ID);
		progressCount.text = string.Format(_localizationService.Text("ui.collections.tab.progress"), unlockedCards, totalCards);
		icon.sprite = tab.Icon;
		hintStream?.Dispose();
		hintStream = cards.OnCardUnlock.Where((ICard _card) => _card.GroupID.Equals(tab.ID)).Subscribe(delegate
		{
			hint.SetActive(value: true);
			unlockedCards = cards.UnlockedCount(tab.ID);
			progressCount.text = string.Format(_localizationService.Text("ui.collections.tab.progress"), unlockedCards, totalCards);
		}).AddTo(this);
	}

	public void OnSelect(bool value)
	{
		if (hint.activeSelf && value)
		{
			hint.SetActive(value: false);
		}
		if (value)
		{
			grid.Display(Source.ID);
		}
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public Memento SaveState()
	{
		return new TabViewMemento(hint.activeSelf, this);
	}

	public void LoadState(Memento memento)
	{
		TabViewMemento tabViewMemento = memento as TabViewMemento;
		hint.SetActive(tabViewMemento.hintActivity);
	}
}
