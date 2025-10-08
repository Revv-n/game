using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Views;

public sealed class SelloutButtonViewInstaller : MonoInstaller<SelloutButtonViewInstaller>
{
	[SerializeField]
	private SelloutButtonView _selloutButtonView;

	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<SelloutButtonView>().FromInstance(_selloutButtonView).AsSingle();
	}
}
