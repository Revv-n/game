using StripClub.UI;
using UnityEngine;
using Zenject;

namespace StripClub.Messenger.UI;

public sealed class ResponseOptionInstaller : MonoInstaller<ResponseOptionInstaller>
{
	[SerializeField]
	private Transform ItemContainer;

	[SerializeField]
	private ItemView ItemViewPrefab;

	public override void InstallBindings()
	{
		((TransformScopeConcreteIdArgConditionCopyNonLazyBinder)((FactoryFromBinderBase)((MonoInstallerBase)this).Container.BindIFactory<ItemView>()).FromComponentInNewPrefab((Object)ItemViewPrefab)).UnderTransform(ItemContainer);
		((FromBinder)((MonoInstallerBase)this).Container.Bind<ItemView.Manager>()).FromNewComponentOn(ItemContainer.gameObject).AsSingle();
	}
}
