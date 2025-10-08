using System;
using GreenT.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

[RequireComponent(typeof(Window))]
public class WindowInputBlock : MonoBehaviour, IInputBlocker
{
	[SerializeField]
	private Window window;

	private IInputBlockerController blockerController;

	protected Subject<IInputBlocker> onUpdate = new Subject<IInputBlocker>();

	public virtual bool IsLaunched => window?.IsOpened ?? false;

	public IObservable<IInputBlocker> OnUpdate => Observable.AsObservable<IInputBlocker>((IObservable<IInputBlocker>)onUpdate);

	[Inject]
	private void Init(IInputBlockerController blockerController)
	{
		this.blockerController = blockerController;
	}

	protected virtual void Awake()
	{
		blockerController.AddBlocker(this);
		window.OnChangeState += SendEvent;
	}

	private void SendEvent(object sender, EventArgs e)
	{
		onUpdate.OnNext((IInputBlocker)this);
	}

	private void OnDestroy()
	{
		if ((bool)window)
		{
			window.OnChangeState -= SendEvent;
		}
		blockerController.Remove(this);
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}
}
