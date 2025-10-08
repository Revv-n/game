using System;
using GreenT.Localizations;
using StripClub.Messenger.UI;
using TMPro;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class PlayerMessageView : MessageView<PlayerChatMessage>
{
	[SerializeField]
	private TMP_Text playerMessage;

	[SerializeField]
	private TMP_Text sendTime;

	[SerializeField]
	private GameObject sendMark;

	[SerializeField]
	private GameObject messageTail;

	private LocalizationService _localizationService;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(PlayerChatMessage message)
	{
		base.Set(message);
		ResponseOption getChoosenResponse = message.GetChoosenResponse;
		if (getChoosenResponse == null)
		{
			throw new Exception("This view must me called when player have already chose the message");
		}
		if (sendMark != null)
		{
			sendMark.SetActive(value: true);
		}
		playerMessage.text = _localizationService.Text(getChoosenResponse.LocalizationKey);
		sendTime.text = message.Time.ToShortTimeString();
	}

	public override void MarkAsLast(bool isLast)
	{
		base.MarkAsLast(isLast);
		messageTail.SetActive(isLast);
	}
}
