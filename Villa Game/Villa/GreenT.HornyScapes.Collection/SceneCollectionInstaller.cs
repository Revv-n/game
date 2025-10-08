using GreenT.HornyScapes.UI;
using StripClub.UI.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Collection;

public class SceneCollectionInstaller : MonoInstaller<SceneCollectionInstaller>
{
	[SerializeField]
	private ToggleGroup tabGroup;

	[SerializeField]
	private CollectionCardView cardPrefab;

	[SerializeField]
	private Transform cardParent;

	[SerializeField]
	private CardsDisplayGrid cardsGrid;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<CollectionCardView>().FromComponentInNewPrefab(cardPrefab).UnderTransform(cardParent)
			.AsCached();
		base.Container.Bind<CollectionCardView.Manager>().FromNewComponentOn(cardParent.gameObject).AsSingle();
		base.Container.Bind<CardsDisplayGrid>().FromInstance(cardsGrid).AsSingle();
	}

	private void OnTabInstantiated(InjectContext arg1, object tabObject)
	{
		(tabObject as MonoBehaviour).GetComponent<Toggle>().group = tabGroup;
	}
}
