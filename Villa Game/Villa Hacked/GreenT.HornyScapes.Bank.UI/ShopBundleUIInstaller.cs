using System;
using System.Runtime.CompilerServices;
using GreenT.HornyScapes.Booster;
using GreenT.HornyScapes.Characters.Skins.UI;
using GreenT.HornyScapes.Extensions;
using GreenT.HornyScapes.Presents.UI;
using GreenT.HornyScapes.UI;
using StripClub.Model.Shop.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class ShopBundleUIInstaller : MonoInstaller
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static BindingCondition _003C_003E9__16_0;

		public static BindingCondition _003C_003E9__16_1;

		public static BindingCondition _003C_003E9__16_2;

		internal bool _003CBindBigCard_003Eb__16_0(InjectContext _context)
		{
			if (_context.ParentContext.Identifier != null)
			{
				return _context.ParentContext.Identifier.Equals("CardsManager");
			}
			return false;
		}

		internal bool _003CBindBigCard_003Eb__16_1(InjectContext _context)
		{
			if (_context.ParentContext.Identifier != null)
			{
				return _context.ParentContext.Identifier.Equals("MergeItemBigCardsManager");
			}
			return false;
		}

		internal bool _003CBindBigCard_003Eb__16_2(InjectContext _context)
		{
			if (_context.ParentContext.Identifier != null)
			{
				return _context.ParentContext.Identifier.Equals("ResourceBigCardsManager");
			}
			return false;
		}
	}

	[Header("Small options")]
	[SerializeField]
	private CurrencyDropView currencyPrefab;

	[SerializeField]
	private CardDropView cardPrefab;

	[SerializeField]
	private MergeItemDropView mergeItemPrefab;

	[SerializeField]
	private SkinDropView skinPrefab;

	[SerializeField]
	private DecorationDropView decorationPrefab;

	[SerializeField]
	private LevelDropView levelDropView;

	[SerializeField]
	private BoosterDropView _boosterPrefab;

	[SerializeField]
	private LootboxDropView lootboxDropView;

	[SerializeField]
	private PresentDropView presentDropView;

	[SerializeField]
	private Transform itemContainer;

	[Space]
	[Header("Big cards")]
	[SerializeField]
	private CardDropView cardViewPrefab;

	[SerializeField]
	private MergeItemDropCardBigView mergeItemDropCardBigPrefab;

	[SerializeField]
	private ResourceDropCardBigView resourceDropCardBigViewPrefab;

	[SerializeField]
	private Transform cardViewContainer;

	public override void InstallBindings()
	{
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<SmallCardsViewManager>()).AsSingle();
		((ScopeConcreteIdArgConditionCopyNonLazyBinder)((MonoInstallerBase)this).Container.Bind<BigCardsViewManager>()).AsSingle();
		((MonoInstallerBase)this).Container.BindViewStructure<CurrencyDropView, CurrencyDropView.Manager>(currencyPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<SkinDropView, SkinDropView.Manager>(skinPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropView, DecorationDropView.Manager>(decorationPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<DecorationDropViewWithRarity, DecorationDropViewWithRarity.Manager>(decorationPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LevelDropView, LevelDropView.Manager>(levelDropView, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<BoosterDropView, BoosterDropView.Manager>(_boosterPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<LootboxDropView, LootboxDropView.Manager>(lootboxDropView, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<MergeItemDropView, MergeItemDropView.Manager>(mergeItemPrefab, itemContainer);
		((MonoInstallerBase)this).Container.BindViewStructure<PresentDropView, PresentDropView.Manager>(presentDropView, itemContainer);
		BindSmallCard();
		BindBigCard();
	}

	private void BindSmallCard()
	{
		((ConditionCopyNonLazyBinder)((FromBinder)((MonoInstallerBase)this).Container.Bind<CardDropView.Manager>()).FromNewComponentOn(itemContainer.gameObject).AsCached()).WhenInjectedInto<SmallCardsViewManager>();
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardDropView>()).FromComponentInNewPrefab((UnityEngine.Object)cardPrefab)).UnderTransform(itemContainer);
	}

	private void BindBigCard()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		((FromBinder)((MonoInstallerBase)this).Container.Bind<CardDropView.Manager>().WithId((object)"CardsManager")).FromNewComponentOn(cardViewContainer.gameObject).AsCached();
		ConcreteIdArgConditionCopyNonLazyBinder obj = ((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<CardDropView>()).FromComponentInNewPrefab((UnityEngine.Object)cardViewPrefab)).UnderTransform(cardViewContainer).AsCached();
		object obj2 = _003C_003Ec._003C_003E9__16_0;
		if (obj2 == null)
		{
			BindingCondition val = (InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("CardsManager");
			_003C_003Ec._003C_003E9__16_0 = val;
			obj2 = (object)val;
		}
		((ConditionCopyNonLazyBinder)obj).When((BindingCondition)obj2);
		((FromBinder)((MonoInstallerBase)this).Container.Bind<MergeItemDropCardBigView.Manager>().WithId((object)"MergeItemBigCardsManager")).FromNewComponentOn(cardViewContainer.gameObject).AsCached();
		ConcreteIdArgConditionCopyNonLazyBinder obj3 = ((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<MergeItemDropCardBigView>()).FromComponentInNewPrefab((UnityEngine.Object)mergeItemDropCardBigPrefab)).UnderTransform(cardViewContainer).AsCached();
		object obj4 = _003C_003Ec._003C_003E9__16_1;
		if (obj4 == null)
		{
			BindingCondition val2 = (InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("MergeItemBigCardsManager");
			_003C_003Ec._003C_003E9__16_1 = val2;
			obj4 = (object)val2;
		}
		((ConditionCopyNonLazyBinder)obj3).When((BindingCondition)obj4);
		((FromBinder)((MonoInstallerBase)this).Container.Bind<ResourceDropCardBigView.Manager>().WithId((object)"ResourceBigCardsManager")).FromNewComponentOn(cardViewContainer.gameObject).AsCached();
		ConcreteIdArgConditionCopyNonLazyBinder obj5 = ((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ResourceDropCardBigView>()).FromComponentInNewPrefab((UnityEngine.Object)resourceDropCardBigViewPrefab)).UnderTransform(cardViewContainer).AsCached();
		object obj6 = _003C_003Ec._003C_003E9__16_2;
		if (obj6 == null)
		{
			BindingCondition val3 = (InjectContext _context) => _context.ParentContext.Identifier != null && _context.ParentContext.Identifier.Equals("ResourceBigCardsManager");
			_003C_003Ec._003C_003E9__16_2 = val3;
			obj6 = (object)val3;
		}
		((ConditionCopyNonLazyBinder)obj5).When((BindingCondition)obj6);
	}
}
