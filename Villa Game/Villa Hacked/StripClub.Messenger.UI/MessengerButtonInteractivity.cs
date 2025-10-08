using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.Messenger.UI;

public class MessengerButtonInteractivity : MonoBehaviour
{
	private IMessengerManager messenger;

	[SerializeField]
	private Button messengerButton;

	[Inject]
	public void Init(IMessengerManager messenger)
	{
		this.messenger = messenger;
		EvalButtonInteractivity();
	}

	public void EvalButtonInteractivity()
	{
		if (messenger == null)
		{
			return;
		}
		bool num = messenger.GetMessages().Any();
		messengerButton.interactable = messenger.GetMessages().Any();
		if (!num)
		{
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MessengerUpdateArgs>(Observable.First<MessengerUpdateArgs>(messenger.OnUpdate, (Func<MessengerUpdateArgs, bool>)((MessengerUpdateArgs _args) => _args.Dialogue != null)), (Action<MessengerUpdateArgs>)delegate
			{
				messengerButton.interactable = true;
			}), (Component)this);
		}
	}
}
