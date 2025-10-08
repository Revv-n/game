using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public sealed class ResponseOptionInstaller : MonoInstaller<ResponseOptionInstaller>
{
	[SerializeField]
	private Transform ItemContainer;

	[SerializeField]
	private ItemView ItemViewPrefab;

	public override void InstallBindings()
	{
		base.Container.BindIFactory<ItemView>().FromComponentInNewPrefab(ItemViewPrefab).UnderTransform(ItemContainer);
		base.Container.Bind<ItemView.Manager>().FromNewComponentOn(ItemContainer.gameObject).AsSingle();
	}
}
