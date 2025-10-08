using GreenT.HornyScapes.Resources.UI;
using GreenT.Model.Reactive;
using StripClub.Model;
using Zenject;

namespace GreenT.UI;

public class WindowOpenerWithResources : WindowOpener
{
	public CurrencyType[] resources = new CurrencyType[3]
	{
		CurrencyType.Soft,
		CurrencyType.Hard,
		CurrencyType.Energy
	};

	private ReactiveCollection<CurrencyType> visibleCurrenciesManager;

	[Inject]
	public void Init(ReactiveCollection<CurrencyType> manager)
	{
		visibleCurrenciesManager = manager;
	}

	public override void Click()
	{
		visibleCurrenciesManager.SetItems(resources);
		base.Click();
		base.WindowsOpener.Get<ResourcesWindow>().Open();
	}
}
