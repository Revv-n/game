using System;
using GreenT.Data;
using UniRx;

namespace StripClub.Messenger;

[MementoHolder]
public abstract class ChatMessage : ISavableState, IDisposable
{
	[Flags]
	public enum MessageState
	{
		None = 0,
		Delivered = 1,
		Read = 2,
		Passed = 4
	}

	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int serialNumber;

		public DateTime deliveryTime;

		public MessageState state;

		public Memento(ChatMessage message)
			: base(message)
		{
			serialNumber = message.SerialNumber;
			deliveryTime = message.Time;
			state = message.State;
		}
	}

	private readonly string uniqueKey;

	protected Subject<ChatMessage> onUpdate = new Subject<ChatMessage>();

	public int DialogueID { get; }

	public int SerialNumber { get; }

	public virtual string LocalizationKey { get; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public DateTime Time { get; private set; }

	public MessageState State { get; protected set; }

	public abstract MessageType MessageType { get; }

	public IObservable<ChatMessage> OnUpdate => onUpdate.AsObservable();

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public ChatMessage(int dialogue_id, int serial_number, DateTime time, MessageState currentState = MessageState.None)
	{
		DialogueID = dialogue_id;
		SerialNumber = serial_number;
		LocalizationKey = "content.chat." + DialogueID + "." + SerialNumber;
		Time = time;
		State = currentState;
		uniqueKey = "Message" + SerialNumber + "." + DialogueID;
	}

	public void AddFlag(MessageState state)
	{
		State |= state;
		OnAddFlag(state);
		onUpdate.OnNext(this);
	}

	protected abstract void OnAddFlag(MessageState state);

	public override string ToString()
	{
		return base.ToString() + "ID: " + DialogueID + "#" + SerialNumber;
	}

	public virtual GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public virtual void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		Time = memento2.deliveryTime;
		State = memento2.state;
		onUpdate.OnNext(this);
	}

	public virtual void Dispose()
	{
		onUpdate.OnCompleted();
		onUpdate.Dispose();
	}
}
