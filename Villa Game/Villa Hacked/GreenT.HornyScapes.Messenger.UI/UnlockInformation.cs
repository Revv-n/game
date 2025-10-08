using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Dates.Extensions;
using GreenT.HornyScapes.Relationships;
using GreenT.HornyScapes.Relationships.Mappers;
using GreenT.HornyScapes.Relationships.Models;
using GreenT.HornyScapes.Relationships.Providers;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.Messenger;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Messenger.UI;

public class UnlockInformation : MonoView<Conversation>
{
	private const string promoteLockerDescription = "ui.messenger.promote_locker.description";

	private const string relationshipLockerDescription = "ui.messenger.promote_locker.description2";

	private const string promoteButtonKey = "ui.messenger.promote_button";

	private const string relationshipButtonKey = "ui.messenger.affection_locker.button.title";

	private const string relationshipLevelKey = "ui.datestatus.{0}";

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private TMP_Text unlockMessage;

	[SerializeField]
	private TMP_Text buttonTitle;

	[SerializeField]
	private Button actionButton;

	[SerializeField]
	private RelationshipVisualData _relationshipVisualData;

	private IMessengerManager _messenger;

	private IOpener<ICard> _promoteWindowOpener;

	private CharacterManager _characterManager;

	private LocalizationService _localizationService;

	private RelationshipMapperProvider _relationshipMapperProvider;

	private RelationshipRewardMapperProvider _relationshipRewardMapperProvider;

	private RelationshipProvider _relationshipProvider;

	private RelationshipOpener _relationshipWindowOpener;

	private IDisposable _updateStream;

	private readonly IDisposable _unlockTextDisposable;

	private readonly CompositeDisposable _disposables = new CompositeDisposable();

	private readonly CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	public void Init(IMessengerManager messenger, IOpener<ICard> promoteWindowOpener, CharacterManager characterManager, LocalizationService localizationService, RelationshipMapperProvider relationshipMapperProvider, RelationshipRewardMapperProvider relationshipRewardMapperProvider, RelationshipProvider relationshipProvider, RelationshipOpener relationshipWindowOpener)
	{
		_messenger = messenger;
		_promoteWindowOpener = promoteWindowOpener;
		_characterManager = characterManager;
		_localizationService = localizationService;
		_relationshipMapperProvider = relationshipMapperProvider;
		_relationshipRewardMapperProvider = relationshipRewardMapperProvider;
		_relationshipProvider = relationshipProvider;
		_relationshipWindowOpener = relationshipWindowOpener;
	}

	public override void Set(Conversation conversation)
	{
		base.Set(conversation);
		TrackInformation(conversation);
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			TrackInformation(base.Source);
		}
	}

	private void TrackInformation(Conversation conversation)
	{
		UpdateInformation();
		_updateStream?.Dispose();
		_updateStream = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Conversation>(conversation.OnUpdate, (Action<Conversation>)delegate
		{
			UpdateInformation();
		}), (Component)this);
	}

	private void UpdateInformation()
	{
		_disposables.Clear();
		RelationshipRewardedLocker relationshipRewaredLocker;
		if (TryGetLockers(out PromoteLocker<ICharacter> promoteLocker, out DialogueCompleteLocker dialogueCompleteLocker))
		{
			ICharacter character = _characterManager.Get(promoteLocker.CardID);
			int level = promoteLocker.Level;
			IConnectableObservable<bool> obj = Observable.Publish<bool>(Observable.CombineLatest<bool, bool, bool>((IObservable<bool>)dialogueCompleteLocker.IsOpen, (IObservable<bool>)promoteLocker.IsOpen, (Func<bool, bool, bool>)((bool isDialogueComplete, bool isCharacterPromoted) => isDialogueComplete && !isCharacterPromoted)));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)obj, (Action<bool>)delegate(bool isActive)
			{
				container.SetActive(isActive);
				unlockMessage.gameObject.SetActive(isActive);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)obj, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
			{
				SetTextFields(character, level);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ReactiveCommandExtensions.BindTo((IReactiveCommand<Unit>)(object)ReactiveCommandExtensions.ToReactiveCommand((IObservable<bool>)obj, true), actionButton), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(actionButton), (Action<Unit>)delegate
			{
				ShowCharacterCard(character);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(obj.Connect(), (ICollection<IDisposable>)_disposables);
		}
		else if (TryGetLockers(out dialogueCompleteLocker, out relationshipRewaredLocker))
		{
			ICharacter character = _characterManager.Get(base.Source.ParticipantIDs[0]);
			int relationshipdId = relationshipRewaredLocker.RelationshipdId;
			RelationshipMapper relationshipMapper = _relationshipMapperProvider.Get(relationshipdId);
			Relationship relationship = _relationshipProvider.Get(relationshipdId);
			int[] rewards = relationshipMapper.rewards;
			int num = rewards.IndexOf(relationshipRewaredLocker.RewardId);
			int num2 = 2;
			int num3 = int.MinValue;
			for (int i = 0; i <= num; i++)
			{
				int status_number = _relationshipRewardMapperProvider.Get(rewards[i]).status_number;
				if (status_number < num3)
				{
					num2++;
				}
				num3 = status_number;
			}
			int status_number2 = _relationshipRewardMapperProvider.Get(relationshipRewaredLocker.RewardId).status_number;
			string statusText = _relationshipVisualData.GetStatusText(num2, status_number2, _localizationService.Text($"ui.datestatus.{num2}"));
			relationship.CheckReward(relationshipRewaredLocker.RewardId);
			IConnectableObservable<bool> obj2 = Observable.Publish<bool>(Observable.CombineLatest<bool, bool, bool>((IObservable<bool>)dialogueCompleteLocker.IsOpen, (IObservable<bool>)relationshipRewaredLocker.IsOpen, (Func<bool, bool, bool>)((bool isDialogueComplete, bool isRewardCollected) => isDialogueComplete && !isRewardCollected)));
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>((IObservable<bool>)obj2, (Action<bool>)delegate(bool isActive)
			{
				container.SetActive(isActive);
				unlockMessage.gameObject.SetActive(isActive);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)obj2, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
			{
				SetTextFields(character, statusText);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ReactiveCommandExtensions.BindTo((IReactiveCommand<Unit>)(object)ReactiveCommandExtensions.ToReactiveCommand((IObservable<bool>)obj2, true), actionButton), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(actionButton), (Action<Unit>)delegate
			{
				ShowRelationship(character);
			}), (ICollection<IDisposable>)_disposables);
			DisposableExtensions.AddTo<IDisposable>(obj2.Connect(), (ICollection<IDisposable>)_disposables);
		}
		else
		{
			container.SetActive(value: false);
		}
	}

	private void SetTextFields(ICharacter character, int level)
	{
		_localizationDisposables.Clear();
		IReadOnlyReactiveProperty<string> obj = _localizationService.ObservableText(character.NameKey);
		IReadOnlyReactiveProperty<string> val = _localizationService.ObservableText("ui.messenger.promote_locker.description");
		IReadOnlyReactiveProperty<string> val2 = _localizationService.ObservableText("ui.messenger.promote_button");
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(Observable.CombineLatest<string, string, string>((IObservable<string>)obj, (IObservable<string>)val, (Func<string, string, string>)((string charName, string desc) => string.Format(desc, charName, level))), (Action<string>)delegate(string message)
		{
			unlockMessage.text = message;
		}), (ICollection<IDisposable>)_localizationDisposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)val2, (Action<string>)delegate(string text)
		{
			buttonTitle.text = text;
		}), (ICollection<IDisposable>)_localizationDisposables);
	}

	private void SetTextFields(ICharacter character, string statusText)
	{
		_localizationDisposables.Clear();
		IReadOnlyReactiveProperty<string> obj = _localizationService.ObservableText(character.NameKey);
		IReadOnlyReactiveProperty<string> val = _localizationService.ObservableText("ui.messenger.promote_locker.description2");
		IReadOnlyReactiveProperty<string> val2 = _localizationService.ObservableText("ui.messenger.affection_locker.button.title");
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(Observable.CombineLatest<string, string, string>((IObservable<string>)obj, (IObservable<string>)val, (Func<string, string, string>)((string charName, string desc) => string.Format(desc, charName, statusText))), (Action<string>)delegate(string message)
		{
			unlockMessage.text = message;
		}), (ICollection<IDisposable>)_localizationDisposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>((IObservable<string>)val2, (Action<string>)delegate(string text)
		{
			buttonTitle.text = text;
		}), (ICollection<IDisposable>)_localizationDisposables);
	}

	public bool TryGetLockers(out PromoteLocker<ICharacter> promoteLocker, out DialogueCompleteLocker dialogueCompleteLocker)
	{
		IEnumerator<DialogueLocker> enumerator = _messenger.GetDialogueLockers().GetEnumerator();
		enumerator.Reset();
		promoteLocker = null;
		dialogueCompleteLocker = null;
		while (enumerator.MoveNext() && promoteLocker == null && dialogueCompleteLocker == null)
		{
			DialogueLocker current = enumerator.Current;
			Dialogue lastDialogue = base.Source.Dialogues.LastOrDefault();
			dialogueCompleteLocker = current.Lockers.OfType<DialogueCompleteLocker>().FirstOrDefault((DialogueCompleteLocker _locker) => _locker.DialogueID == lastDialogue.ID);
			if (dialogueCompleteLocker != null)
			{
				promoteLocker = current.Lockers.OfType<PromoteLocker<ICharacter>>().FirstOrDefault();
			}
		}
		if (promoteLocker != null)
		{
			return dialogueCompleteLocker != null;
		}
		return false;
	}

	public bool TryGetLockers(out DialogueCompleteLocker dialogueCompleteLocker, out RelationshipRewardedLocker relationshipRewaredLocker)
	{
		IEnumerator<DialogueLocker> enumerator = _messenger.GetDialogueLockers().GetEnumerator();
		enumerator.Reset();
		dialogueCompleteLocker = null;
		relationshipRewaredLocker = null;
		while (enumerator.MoveNext() && dialogueCompleteLocker == null && relationshipRewaredLocker == null)
		{
			DialogueLocker current = enumerator.Current;
			Dialogue lastDialogue = base.Source.Dialogues.LastOrDefault();
			dialogueCompleteLocker = current.Lockers.OfType<DialogueCompleteLocker>().FirstOrDefault((DialogueCompleteLocker _locker) => _locker.DialogueID == lastDialogue.ID);
			if (dialogueCompleteLocker != null)
			{
				relationshipRewaredLocker = current.Lockers.OfType<RelationshipRewardedLocker>().FirstOrDefault();
			}
		}
		if (dialogueCompleteLocker != null)
		{
			return relationshipRewaredLocker != null;
		}
		return false;
	}

	public void ShowCharacterCard(ICard card)
	{
		_promoteWindowOpener.Open(card);
	}

	public void ShowRelationship(ICard card)
	{
		_relationshipWindowOpener.Open(card);
	}

	private void OnDisable()
	{
		_disposables.Clear();
		_updateStream?.Dispose();
		_unlockTextDisposable?.Dispose();
	}

	private void OnDestroy()
	{
		CompositeDisposable disposables = _disposables;
		if (disposables != null)
		{
			disposables.Dispose();
		}
		_unlockTextDisposable?.Dispose();
	}
}
