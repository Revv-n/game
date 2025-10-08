using GreenT.Net.User;
using Zenject;

namespace GreenT.HornyScapes.Net;

public class UserRequestsListenerInstaller : Installer<UserRequestsListenerInstaller>
{
	public override void InstallBindings()
	{
		base.Container.BindInterfacesAndSelfTo<UserRequestsListener>().AsSingle();
	}
}
