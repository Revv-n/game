using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

public abstract class BaseNotify : MonoBehaviour
{
	[SerializeField]
	private GameObject notify;

	protected CompositeDisposable notifyStream = new CompositeDisposable();

	public bool IsActive { get; protected set; }

	public void SetState(bool activate)
	{
		IsActive = activate;
		notify.SetActive(IsActive);
	}

	protected virtual void ListenEvents()
	{
		notifyStream.Clear();
	}

	protected virtual void OnDestroy()
	{
		notifyStream.Dispose();
	}
}
