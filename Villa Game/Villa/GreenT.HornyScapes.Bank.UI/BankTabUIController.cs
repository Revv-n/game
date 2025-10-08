using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using StripClub.UI;
using StripClub.UI.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class BankTabUIController : MonoView<BankTab.Manager>
{
	private ReactiveProperty<BankTab> currentTab = new ReactiveProperty<BankTab>();

	private CompositeDisposable toggleStream = new CompositeDisposable();

	private IDisposable visibleTabsStream;

	private List<BankTabView> activeTabs = new List<BankTabView>();

	private IViewManager<BankTab, BankTabView> tabUIManager;

	public IReadOnlyReactiveProperty<BankTab> SelectedTab => currentTab.ToReadOnlyReactiveProperty();

	[Inject]
	public void Init(IViewManager<BankTab, BankTabView> tabUIManager)
	{
		this.tabUIManager = tabUIManager;
	}

	public override void Set(BankTab.Manager source)
	{
		base.Set(source);
		if (base.isActiveAndEnabled)
		{
			UpdateView();
		}
	}

	public void Initialize()
	{
		UpdateView();
		BankTabView bankTabView = activeTabs.First((BankTabView x) => x);
		bankTabView.TabToggle.isOn = true;
		currentTab.Value = bankTabView.Source;
	}

	public void UpdateView()
	{
		IObservable<BankTab> source = base.Source.Collection.ToObservable().SelectMany((Func<BankTab, IObservable<BankTab>>)EmitOnUnlock);
		IEnumerable<BankTab> tabs = base.Source.Collection.Where((BankTab _tab) => _tab.Lock.IsOpen.Value);
		LoadTabs(tabs);
		visibleTabsStream?.Dispose();
		visibleTabsStream = source.Subscribe(ShowTab).AddTo(this);
		static IObservable<BankTab> EmitOnUnlock(BankTab tab)
		{
			return from _ in tab.Lock.IsOpen.Skip(1)
				select tab;
		}
	}

	private void LoadTabs(IEnumerable<BankTab> tabs)
	{
		toggleStream.Clear();
		activeTabs.Clear();
		tabUIManager.HideAll();
		activeTabs.AddRange(tabs.Select(tabUIManager.Display));
		activeTabs.Select(OnTabShow).Merge().Subscribe((Action<BankTab>)delegate(BankTab _tab)
		{
			currentTab.Value = _tab;
		}, (Action<Exception>)delegate
		{
		})
			.AddTo(toggleStream);
	}

	private void ShowTab(BankTab tab)
	{
		bool value = tab.Lock.IsOpen.Value;
		BankTabView view;
		bool flag = TryGetActiveTabView(tab, out view);
		if (value && !flag)
		{
			view = tabUIManager.Display(tab);
			activeTabs.Add(view);
		}
		else if (!value && flag)
		{
			view.Display(display: false);
			activeTabs.Remove(view);
		}
	}

	public void SelectTabBySignal(OpenTabSignal signal)
	{
		BankTab bankTab = base.Source.Collection.FirstOrDefault((BankTab _tab) => _tab.ID == signal.TabID);
		if (bankTab != null)
		{
			if (!TryGetActiveTabView(bankTab, out var view))
			{
				view = activeTabs.First();
			}
			view.TabToggle.isOn = true;
			currentTab.Value = view.Source;
		}
	}

	private bool TryGetActiveTabView(BankTab tab, out BankTabView view)
	{
		view = activeTabs.FirstOrDefault((BankTabView _tab) => _tab.Source.ID == tab.ID);
		_ = view == null;
		return view != null;
	}

	private IObservable<BankTab> OnTabShow(BankTabView tabView)
	{
		return (from x in tabView.TabToggle.OnValueChangedAsObservable()
			where x
			select x into _
			select tabView.Source).Share();
	}

	private void OnDisable()
	{
		visibleTabsStream?.Dispose();
		toggleStream.Clear();
	}

	private void OnDestroy()
	{
		toggleStream.Dispose();
	}
}
