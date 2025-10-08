using System;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Events.Content;
using GreenT.HornyScapes.Resources.UI;
using GreenT.Model.Reactive;
using GreenT.UI;
using StripClub.Model;
using StripClub.UI.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BankWindow : Window
{
	[SerializeField]
	private Button close;

	private GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrenciesManager;

	private SectionController sectionController;

	private RouletteSectionController rouletteSectionController;

	private BankTabUIController tabUIController;

	private ContentSelectorGroup contentSelectorGroup;

	private IDisposable tabSelectStream;

	private CurrencyType[] prevWindowsVisibleCurrencies;

	private bool waitForUpdate;

	[Inject]
	private void Init(GreenT.Model.Reactive.ReactiveCollection<CurrencyType> manager, SectionController sectionController, BankTabUIController tabUIController, ContentSelectorGroup contentSelectorGroup, RouletteSectionController rouletteSectionController)
	{
		visibleCurrenciesManager = manager;
		this.sectionController = sectionController;
		this.rouletteSectionController = rouletteSectionController;
		this.tabUIController = tabUIController;
		this.contentSelectorGroup = contentSelectorGroup;
	}

	private void OnEnable()
	{
		close.OnClickAsObservable().TakeUntilDisable(this).Subscribe(delegate
		{
			Close();
		})
			.AddTo(this);
		Initialize();
	}

	protected virtual void OnDisable()
	{
		tabSelectStream?.Dispose();
	}

	public override void Open()
	{
		if (!IsOpened)
		{
			prevWindowsVisibleCurrencies = visibleCurrenciesManager.ToArray();
			visibleCurrenciesManager.SetItems((contentSelectorGroup.Current != 0) ? CurrencyType.Event : CurrencyType.Soft, CurrencyType.Hard);
		}
		base.Open();
		windowsManager.Get<ResourcesWindow>().Open();
	}

	public override void Close()
	{
		visibleCurrenciesManager.SetItems(prevWindowsVisibleCurrencies);
		base.Close();
		windowsManager.Get<ResourcesWindow>().Open();
	}

	internal void OnUpdateRequest(ViewUpdateSignal viewUpdateSignal)
	{
		if (!waitForUpdate)
		{
			waitForUpdate = true;
			Observable.TimerFrame(1).Subscribe((Action<long>)delegate
			{
				tabUIController.UpdateView();
			}, (Action)delegate
			{
				waitForUpdate = false;
			}).AddTo(this);
		}
	}

	private void Initialize()
	{
		tabUIController.Initialize();
		tabSelectStream?.Dispose();
		tabSelectStream = tabUIController.SelectedTab.Where((BankTab _tab) => _tab != null).Subscribe(delegate(BankTab value)
		{
			rouletteSectionController.ForceHideAll();
			sectionController.ForceHideAll();
			if (value.Layout == LayoutType.Roulette)
			{
				rouletteSectionController.LoadSection(value);
			}
			else
			{
				sectionController.LoadSection(value);
			}
		}).AddTo(this);
	}
}
