using GreenT.HornyScapes.Bank.BankTabs;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MergeStore;

public class MergeStoreView : BankSectionView
{
	[SerializeField]
	private MergeStoreSectionView _regular;

	[SerializeField]
	private MergeStoreSectionView _premium;

	private MergeStoreService _service;

	[Inject]
	public void SetStoreService(MergeStoreService service)
	{
		_service = service;
	}

	public override void Set(BankTab settings)
	{
		base.Set(settings);
		StorePreset sections = _service.GetSections();
		_regular.Set(sections.Regular);
		_premium.Set(sections.Premium);
	}
}
