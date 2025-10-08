using UnityEngine;

namespace StripClub.UI;

public class ReverseFunctions : MonoBehaviour
{
	public void SetInactive(bool inactive)
	{
		base.gameObject.SetActive(!inactive);
	}
}
