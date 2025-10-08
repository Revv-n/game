using GreenT.HornyScapes.Bank;
using StripClub.Model.Shop;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public abstract class AbstractSectionController<TKey, TSection, TView> : MonoBehaviour where TSection : IBankSection where TView : IView<TSection>
{
	protected AbstractSectionManager<TKey, TSection, TView> sectionManager;

	public TSection Source { get; private set; }

	public TView CurrentSection { get; private set; }

	[Inject]
	public void Init(AbstractSectionManager<TKey, TSection, TView> sectionManager)
	{
		this.sectionManager = sectionManager;
	}

	public virtual void LoadSection(TSection model)
	{
		Source = model;
		CurrentSection = GetSection();
		CurrentSection.Set(Source);
	}

	public virtual void ForceLoadSection(TSection model)
	{
		Source = model;
		CurrentSection = GetSection();
		(CurrentSection as BaseAbstractLotSectionView<TSection, Lot>).ForceSet(Source);
	}

	public void ForceHideAll()
	{
		sectionManager.HideAll();
	}

	protected abstract TView GetSection();
}
