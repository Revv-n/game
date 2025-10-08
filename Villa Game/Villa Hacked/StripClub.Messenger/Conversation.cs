using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using UniRx;
using UnityEngine;

namespace StripClub.Messenger;

public sealed class Conversation : IDisposable
{
	public readonly int[] ParticipantIDs;

	private readonly bool _namesVisibility;

	private readonly List<Dialogue> _dialogues;

	private readonly CompositeDisposable _dialoguesUpdateStream = new CompositeDisposable();

	private readonly Subject<Conversation> _onUpdate = new Subject<Conversation>();

	private readonly Subject<Conversation> _onHaveNewUpdate = new Subject<Conversation>();

	private readonly Dictionary<int, int> _messagesCountByDialogueId = new Dictionary<int, int>();

	public CharacterManager CharacterManager { get; }

	public int ID { get; private set; }

	public string NameKey { get; private set; }

	public Sprite Icon { get; private set; }

	public int TotalDialoguesAmount { get; private set; }

	public int TotalMessages { get; private set; }

	public int CurrentMessages { get; private set; }

	public int TotalMedia { get; private set; }

	public int CurrentMedia => Dialogues.Sum((Dialogue _dialogue) => _dialogue.CurrentMediaAmount);

	public int UnreadMessagesCount => _dialogues.Sum((Dialogue dialogue) => dialogue.UnreadMessagesCount);

	public int DialoguesFinished => _dialogues.Count((Dialogue dialogue) => dialogue.IsComplete);

	public bool IsComplete => _dialogues.All((Dialogue _dialogue) => _dialogue.IsComplete);

	public IEnumerable<Dialogue> Dialogues => _dialogues;

	public IObservable<Conversation> OnUpdate => (IObservable<Conversation>)_onUpdate;

	public IObservable<Conversation> OnHaveNewUpdate => (IObservable<Conversation>)_onHaveNewUpdate;

	public DateTime LastTimeUpdate => _dialogues.Last().LastTimeUpdate;

	public Conversation(CharacterManager characterManager, int id, int totalDialoguesAmount, int totalMedia, int[] participantIDs, bool namesVisibility, string nameKey = null, Sprite icon = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		CharacterManager = characterManager;
		ID = id;
		TotalDialoguesAmount = totalDialoguesAmount;
		TotalMedia = totalMedia;
		ParticipantIDs = participantIDs;
		_namesVisibility = namesVisibility;
		NameKey = nameKey;
		Icon = icon;
		_dialogues = new List<Dialogue>();
	}

	private void OnAddDialogue(Dialogue dialogue)
	{
		TotalMessages += dialogue.TotalMessages;
		OnUpdateDialogue(dialogue);
	}

	private void OnUpdateDialogue(Dialogue dialogue)
	{
		_messagesCountByDialogueId[dialogue.ID] = dialogue.CurrentMessageAmount;
		foreach (CharacterChatMessage item in dialogue.Messages.Where((ChatMessage p) => p.MessageType == MessageType.Character).OfType<CharacterChatMessage>())
		{
			item.SetNamesVisibility(_namesVisibility);
		}
		CurrentMessages = _messagesCountByDialogueId.Sum((KeyValuePair<int, int> _pair) => _pair.Value);
		_onHaveNewUpdate.OnNext(this);
	}

	public void AddRange(IEnumerable<Dialogue> dialogues)
	{
		foreach (Dialogue dialogue in dialogues)
		{
			_dialogues.Add(dialogue);
			OnAddDialogue(dialogue);
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Dialogue>(dialogue.OnUpdate, (Action<Dialogue>)OnUpdateDialogue), (ICollection<IDisposable>)_dialoguesUpdateStream);
		}
		_onUpdate.OnNext(this);
	}

	public Sprite GetIcon()
	{
		if (Icon != null)
		{
			return Icon;
		}
		CharacterChatMessage characterChatMessage = Dialogues.First().Messages.OfType<CharacterChatMessage>().FirstOrDefault();
		if (characterChatMessage == null)
		{
			return null;
		}
		return CharacterManager.Get(characterChatMessage.CharacterID).GetBundleData().MessengerAvatar;
	}

	public void Dispose()
	{
		_dialoguesUpdateStream.Dispose();
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + ID;
	}
}
