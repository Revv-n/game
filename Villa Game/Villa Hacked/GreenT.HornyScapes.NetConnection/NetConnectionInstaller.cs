using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.NetConnection;

public class NetConnectionInstaller : MonoInstaller<NetConnectionInstaller>
{
	public DisplayNetConnection PopupPrefab;

	public Canvas Canvas;

	private const int IntervalTime = 60;

	public override void InstallBindings()
	{
	}
}
