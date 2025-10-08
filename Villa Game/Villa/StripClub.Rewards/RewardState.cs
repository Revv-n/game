using UnityEngine;

namespace StripClub.Rewards;

public abstract class RewardState : MonoBehaviour
{
	public delegate void EventHandler();

	public event EventHandler Finished;

	protected void Finish()
	{
		this.Finished?.Invoke();
		base.gameObject.SetActive(value: false);
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}
}
