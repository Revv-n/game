using GreenT.HornyScapes.MergeCore.Inventory;
using UnityEngine;
using Zenject;

namespace Merge;

public class InventoryInstaller : MonoInstaller
{
	[SerializeField]
	private MergeInventoryWindow window;

	public override void InstallBindings()
	{
		((FromBinderGeneric<MergeInventoryWindow>)(object)((MonoInstallerBase)this).Container.Bind<MergeInventoryWindow>()).FromInstance(window);
	}
}
