using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Characters.Skins.Content;
using Merge;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace StripClub.UI.Shop;

public sealed class PromoContentView : MonoView
{
	[SerializeField]
	private PromoCardView _anySmallCardTemplate;

	[SerializeField]
	private PromoCardView _girlSmallCardTemplate;

	[SerializeField]
	private PromoCardView _anyBigCard;

	[SerializeField]
	private PromoCardView _girlBigCard;

	[SerializeField]
	private Transform _smallCardsRoot;

	[SerializeField]
	private Transform _bigCardsRoot;

	private DiContainer _container;

	[Inject]
	private void Init(DiContainer container)
	{
		_container = container;
	}

	public void Set(List<LinkedContent> content)
	{
		_smallCardsRoot.gameObject.SetActive(value: false);
		_bigCardsRoot.gameObject.SetActive(value: false);
		if (content.Any() && content != null)
		{
			LinkedContent linkedContent = content.First();
			PromoCardView obj = ((linkedContent is SkinLinkedContent || linkedContent is CardLinkedContent) ? _girlBigCard : _anyBigCard);
			obj.SetActive(active: true);
			obj.Set(linkedContent);
			for (int i = 1; i < content.Count; i++)
			{
				LinkedContent linkedContent2 = content[i];
				PromoCardView promoCardView = ((linkedContent2 is SkinLinkedContent || linkedContent2 is CardLinkedContent) ? _girlSmallCardTemplate : _anySmallCardTemplate);
				_container.InstantiatePrefab((Object)promoCardView, _smallCardsRoot).GetComponent<PromoCardView>().Set(linkedContent2);
			}
			_smallCardsRoot.gameObject.SetActive(value: true);
			_bigCardsRoot.gameObject.SetActive(value: true);
		}
	}
}
