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
		base.Container.Bind<MergeInventoryWindow>().FromInstance(window);
	}
}
