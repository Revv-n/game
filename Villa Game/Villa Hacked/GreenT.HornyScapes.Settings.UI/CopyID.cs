using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Settings.UI;

public class CopyID : MonoBehaviour
{
	private User user;

	[Inject]
	public void Init(User user)
	{
		this.user = user;
	}

	public void Copy()
	{
		CopyUtil.Copy(user.PlayerID);
	}
}
