using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.Presents.Services;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.Relationships.Windows;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.ToolTips;
using GreenT.HornyScapes.UI;
using StripClub.Model.Cards;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class PromoteWindow : PopupWindow, ICardView
{
	[Serializable]
	public class CardViewSettings
	{
		[field: SerializeField]
		public string TitleKey { get; private set; }

		[field: SerializeField]
		public int TargetGroup { get; private set; }

		[field: SerializeField]
		public List<CharacterView> Views { get; private set; }
	}

	[SerializeField]
	private Button close;

	[SerializeField]
	private List<CardView> cardViews;

	[SerializeField]
	private List<CardViewSettings> settingsList;

	[SerializeField]
	private GameObject switchOpenersContainer;

	[SerializeField]
	private ToolTipUIOpener[] switchersOpeners;

	[SerializeField]
	private AutoScrollToggle _scrollToggle;

	[Header("Relationships")]
	[SerializeField]
	private RelationshipUiSetter _relationshipUiSetter;

	private CardsCollection cards;

	private PromoteTab.Manager tabViewManager;

	private CharacterSettingsManager characterManager;

	private SkinManager skinManager;

	private RelationshipProvider _relationshipProvider;

	private PresentsService _presentsService;

	private Relationship _relationship;

	private IDisposable disposable;

	private IDisposable _relationshipStream;

	public ICard Card { get; private set; }

	public CharacterSettings Character { get; private set; }

	[Inject]
	public void Init(CardsCollection cardsCollection, PromoteTab.Manager tabViewManager, CharacterSettingsManager characterManager, SkinManager skinManager, RelationshipProvider relationshipProvider, PresentsService presentsService)
	{
		cards = cardsCollection;
		this.tabViewManager = tabViewManager;
		this.characterManager = characterManager;
		this.skinManager = skinManager;
		_relationshipProvider = relationshipProvider;
		_presentsService = presentsService;
	}

	private void OnEnable()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.TakeUntilDisable<Unit>(UnityUIComponentExtensions.OnClickAsObservable(close), (Component)this), (Action<Unit>)delegate
		{
			Close();
		}), (Component)this);
	}

	public override void Open()
	{
		ResourcesWindow window = windowsManager.Get<ResourcesWindow>();
		windowsManager.Open(window);
		base.Open();
	}

	public void Set(ICard card)
	{
		Card = card;
		Character = characterManager.Get(Card.ID);
		foreach (CardView cardView in cardViews)
		{
			cardView.Set(Card);
		}
		SetRelationship();
		LoadTabs(Character);
		_scrollToggle.Set();
		IPromote promoteOrDefault = cards.GetPromoteOrDefault(Card);
		if (promoteOrDefault != null)
		{
			promoteOrDefault.IsNew.Value = false;
		}
		int[] array = Character.Public.GetBundleData().Avatars.Keys.ToArray();
		SwitchOpenersState(array.Length > 1);
		for (int i = 0; i < array.Length; i++)
		{
			switchersOpeners[i].SetArguments(array[i]);
		}
	}

	public void ActivateTab(int tabId)
	{
		PromoteTab promoteTab = tabViewManager.VisibleViews.FirstOrDefault((PromoteTab tab) => tab.ID == tabId);
		if (promoteTab != null)
		{
			promoteTab.Toggle.isOn = true;
		}
	}

	private void SwitchOpenersState(bool show)
	{
		switchOpenersContainer.SetActive(show);
	}

	private void LoadTabs(CharacterSettings card)
	{
		disposable?.Dispose();
		_relationshipStream?.Dispose();
		tabViewManager.HideAll();
		IObservable<int> observable = Observable.Empty<int>();
		for (int i = 0; i != settingsList.Count; i++)
		{
			CardViewSettings cardViewSettings = settingsList[i];
			if (cardViewSettings.TargetGroup == card.Public.GroupID)
			{
				PromoteTab view = tabViewManager.GetView();
				view.Init(cardViewSettings.TitleKey, i);
				bool flag = cardViewSettings.TitleKey.Contains("skin");
				bool flag2 = cardViewSettings.TitleKey.Contains("dates");
				view.SetLock((flag && !skinManager.Collection.Any((Skin _skin) => _skin.GirlID == card.Public.ID)) || (flag2 && _relationship == null));
				observable = Observable.Merge<int>(observable, new IObservable<int>[1] { view.OnActivate });
				if (flag2 && _relationship != null)
				{
					_relationshipStream = ObservableExtensions.Subscribe<bool>((IObservable<bool>)_relationship.WasComingSoonDates, (Action<bool>)view.ActivateIndicator);
				}
			}
		}
		PromoteTab promoteTab = tabViewManager.VisibleViews.First();
		disposable = ObservableExtensions.Subscribe<int>(observable, (Action<int>)LoadSettings);
		promoteTab.Toggle.isOn = true;
	}

	private void LoadSettings(int tabID)
	{
		foreach (CharacterView item in settingsList.SelectMany((CardViewSettings _settings) => _settings.Views).Distinct())
		{
			if (settingsList[tabID].Views.Contains(item))
			{
				item.gameObject.SetActive(value: true);
				item.Set(Character);
			}
			else
			{
				item.gameObject.SetActive(value: false);
			}
		}
	}

	private void SetRelationship()
	{
		GreenT.HornyScapes.Characters.CharacterInfo characterInfo = Card as GreenT.HornyScapes.Characters.CharacterInfo;
		int relationsipId = characterInfo.RelationsipId;
		_relationship = _relationshipProvider.Get(relationsipId);
		if (_relationship != null)
		{
			_presentsService.Set(characterInfo, _relationship);
			_relationshipUiSetter.Set((characterInfo, _relationship));
		}
	}
}
