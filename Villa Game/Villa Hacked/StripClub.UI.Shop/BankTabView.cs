using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.DebugInfo;
using GreenT.Localizations;
using StripClub.Extensions;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class BankTabView : MonoView<BankTab>
{
	[Flags]
	public enum Mark
	{
		New = 1,
		Free = 2
	}

	[Serializable]
	public class MarkNameDictionary : SerializableDictionary<Mark, string>
	{
	}

	public class Manager : MonoViewManager<BankTab, BankTabView>
	{
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private StatableComponent markState;

	[SerializeField]
	private TextMeshProUGUI[] titles;

	[SerializeField]
	private StatableComponent[] titlesState;

	[SerializeField]
	private GameObject noTimerObject;

	[SerializeField]
	private GameObject unactiveTabTimerPlaceholder;

	[SerializeField]
	private GameObject activeTabTimerPlaceholder;

	[SerializeField]
	private TimerContainer timerContainer;

	[SerializeField]
	private Toggle toggle;

	[SerializeField]
	private DebugInfoContainer _debugInfo;

	protected LotManager lotManager;

	private LocalizationService _localizationService;

	private TimeHelper timeHelper;

	public ReactiveProperty<BankTab> OnSet = new ReactiveProperty<BankTab>();

	private List<Lot> sectionAvailableLots = new List<Lot>();

	private CompositeDisposable lotStreams = new CompositeDisposable();

	private const int FIRST_PURCHASE_TAB_ID = 1001;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	public Toggle TabToggle => toggle;

	public Mark TabMark { get; private set; }

	[Inject]
	public void Init(LotManager lotManager, LocalizationService localizationService, TimeHelper timeHelper)
	{
		this.lotManager = lotManager;
		_localizationService = localizationService;
		this.timeHelper = timeHelper;
	}

	private void Awake()
	{
		if (toggle.group == null)
		{
			toggle.group = toggle.transform.parent.GetComponent<ToggleGroup>();
		}
		noTimerObject.SetActive(value: true);
		toggle.onValueChanged.AddListener(SetTimerContainer);
	}

	private void OnDisable()
	{
		lotStreams.Dispose();
	}

	private void OnDestroy()
	{
		toggle.onValueChanged.RemoveListener(SetTimerContainer);
		CompositeDisposable localizationDisposables = _localizationDisposables;
		if (localizationDisposables != null)
		{
			localizationDisposables.Dispose();
		}
	}

	private void SetTimer()
	{
		timerContainer.SetEnableFromTimer(((CompositeLocker)base.Source.Lock).Lockers);
	}

	private void SetTimerContainer(bool isTabActive)
	{
		GameObject placeholder = (isTabActive ? activeTabTimerPlaceholder : unactiveTabTimerPlaceholder);
		timerContainer.SetPlaceholder(placeholder);
	}

	public void SetTabState(bool active)
	{
		StatableComponent[] array = titlesState;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(active ? 1 : 0);
		}
	}

	public void SetMarks(Mark marks)
	{
		TabMark = marks;
		markState.gameObject.SetActive(marks > (Mark)0);
		if ((marks & Mark.Free) == Mark.Free)
		{
			markState.Set(1);
		}
		else if ((marks & Mark.New) == Mark.New)
		{
			markState.Set(0);
		}
	}

	public override void Set(BankTab tab)
	{
		base.Set(tab);
		SetIcon(base.Source);
		SetTimer();
		_localizationDisposables.Clear();
		IReadOnlyReactiveProperty<string> val = _localizationService.ObservableText(base.Source.LocalizationKey);
		TextMeshProUGUI[] array = titles;
		foreach (TextMeshProUGUI title in array)
		{
			title.text = _localizationService.Text(base.Source.LocalizationKey);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)val, (Action<string>)delegate(string value)
			{
				title.text = value;
			}), (ICollection<IDisposable>)_localizationDisposables);
		}
		GetAvailableSectionLot();
		TabMark = EvaluateMarks();
		SetMarks(TabMark);
		OnSet.SetValueAndForceNotify(tab);
		TryDebug(tab);
	}

	private void TryDebug(BankTab tab)
	{
	}

	private void GetAvailableSectionLot()
	{
		lotStreams.Clear();
		if (base.Source.ID == 1001)
		{
			sectionAvailableLots = lotManager.Collection.Where((Lot _lot) => _lot.TabID == base.Source.ID).ToList();
			Lot lot2 = sectionAvailableLots.First();
			if (lot2 == null)
			{
				return;
			}
			ObservableExtensions.Subscribe<bool>((IObservable<bool>)lot2.Locker.IsOpen, (Action<bool>)delegate(bool isOpen)
			{
				if (isOpen)
				{
					TabMark = EvaluateMarks();
					SetMarks(TabMark);
				}
			});
			return;
		}
		sectionAvailableLots = lotManager.Collection.Where((Lot _lot) => _lot.TabID == base.Source.ID && _lot.IsAvailable()).ToList();
		sectionAvailableLots.ForEach(delegate(Lot lot)
		{
			lotStreams.Add(ObservableExtensions.Subscribe<Lot>(lot.OnLotReceived(), (Action<Lot>)delegate
			{
				TabMark = EvaluateMarks();
				SetMarks(TabMark);
			}));
		});
	}

	private Mark EvaluateMarks()
	{
		Mark mark = (Mark)0;
		if (sectionAvailableLots.Any((Lot _lot) => !_lot.IsViewed))
		{
			mark |= Mark.New;
		}
		if (base.Source.Layout == LayoutType.BundlesChain)
		{
			Lot lot = (lotManager.Collection.Where((Lot _lot) => _lot.TabID == base.Source.ID && _lot.IsAvailable())?.OrderBy((Lot _) => _.SerialNumber))?.FirstOrDefault();
			if (sectionAvailableLots.Any() && lot != null && lot.IsFree)
			{
				mark |= Mark.Free;
			}
		}
		else if (sectionAvailableLots.Any((Lot _lot) => _lot.IsFree && _lot.IsAvailable()))
		{
			mark |= Mark.Free;
		}
		return mark;
	}

	private void SetIcon(BankTab tab)
	{
		Sprite iconSprite = tab.GetIconSprite();
		_ = iconSprite == null;
		icon.sprite = iconSprite;
	}
}
