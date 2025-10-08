using System;
using System.Text;
using GreenT.HornyScapes.Characters;
using GreenT.Localizations;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Messenger.UI;

public class TypingMessageView : MessageView<CharacterChatMessage>
{
	[SerializeField]
	private TMP_Text message;

	[SerializeField]
	private float animationFrequency = 2f;

	private CharacterManager characterManager;

	private LocalizationService _localizationService;

	private StringBuilder builder = new StringBuilder();

	private string characterName;

	private IDisposable animationStream;

	[Inject]
	public void Init(CharacterManager characterManager, LocalizationService localizationService)
	{
		this.characterManager = characterManager;
		_localizationService = localizationService;
	}

	private void OnEnable()
	{
		animationStream?.Dispose();
		animationStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<long>(Observable.DoOnSubscribe<long>(Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(animationFrequency)), (Action)delegate
		{
			builder.Clear();
		}), (Action<long>)delegate
		{
			AnimateTyping();
		}), (Component)this);
	}

	private void OnDisable()
	{
		animationStream?.Dispose();
	}

	public override void Set(CharacterChatMessage message)
	{
		base.Set(message);
		ICharacter character = characterManager.Get(message.CharacterID);
		characterName = _localizationService.Text(character.NameKey);
	}

	private void AnimateTyping()
	{
		if (builder.Length > 3)
		{
			builder.Clear();
		}
		message.text = builder.ToString();
		builder.Append('.');
	}

	public override void Display(bool display)
	{
		base.Display(display);
	}
}
