using UnityEngine;

namespace GreenT.HornyScapes.Registration;

public class GameObjectLoginIndicator : LoginIndicator
{
	[SerializeField]
	private bool activeIfLogged = true;

	protected override void SetLogged(bool isLogged)
	{
		bool active = (activeIfLogged ? isLogged : (!isLogged));
		base.gameObject.SetActive(active);
	}
}
