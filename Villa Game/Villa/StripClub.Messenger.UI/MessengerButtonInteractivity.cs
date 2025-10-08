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
			messenger.OnUpdate.First((MessengerUpdateArgs _args) => _args.Dialogue != null).Subscribe(delegate
			{
				messengerButton.interactable = true;
			}).AddTo(this);
		}
	}
}
