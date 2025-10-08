using GreenT.Model.Collections;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.BankTabs;

public class BankTab : IBankSection
{
	public class Manager : SimpleManager<BankTab>
	{
	}

	private const string localizationPrefix = "content.shop.tab.";

	private readonly BundlesProviderBase _bundlesProvider;

	private readonly ContentSource _contentSource;

	public int ID { get; }

	public string IconPath { get; }

	public int SerialNumber { get; set; }

	public string LocalizationKey { get; }

	public LayoutType Layout { get; }

	public ILocker Lock { get; }

	public LayoutSettings LayoutSets { get; }

	public BankTab(int id, int serialNumber, LayoutType layoutType, string iconPath, ILocker locker)
	{
		ID = id;
		IconPath = iconPath;
		LocalizationKey = "content.shop.tab." + ID;
		SerialNumber = serialNumber;
		Layout = layoutType;
		Lock = locker;
	}

	public BankTab(int id, int serialNumber, LayoutType layoutType, BundlesProviderBase bundlesProvider, ContentSource contentSource, string iconPath, ILocker locker, LayoutSettings layoutParams)
		: this(id, serialNumber, layoutType, iconPath, locker)
	{
		LayoutSets = layoutParams;
		_bundlesProvider = bundlesProvider;
		_contentSource = contentSource;
	}

	public Sprite GetIconSprite()
	{
		return _bundlesProvider.TryFindInConcreteBundle<Sprite>(_contentSource, IconPath);
	}

	public SlotsSectionSettings GetSettings(string path)
	{
		return _bundlesProvider.TryFindInConcreteBundle<SlotsSectionSettings>(_contentSource, path);
	}
}
