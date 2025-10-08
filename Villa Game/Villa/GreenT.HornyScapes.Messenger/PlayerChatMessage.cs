using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Extensions;
using StripClub.Messenger;
using UnityEngine;

namespace GreenT.HornyScapes.Messenger;

[MementoHolder]
public sealed class PlayerChatMessage : ChatMessage
{
	[Serializable]
	public new class Memento : ChatMessage.Memento
	{
		[SerializeField]
		private int chosenReplyID;

		public int ChosenReplyID => chosenReplyID;

		public Memento(PlayerChatMessage message)
			: base(message)
		{
			chosenReplyID = message.chosenReplyID;
		}
	}

	private int chosenReplyID = -1;

	private List<ResponseOption> options;

	public override MessageType MessageType => MessageType.Player;

	public IEnumerable<ResponseOption> GetAvailableOptions => Enumerable.AsEnumerable(options);

	public ResponseOption GetChoosenResponse => options.SingleOrDefault((ResponseOption option) => option.ID == chosenReplyID);

	public PlayerChatMessage(int dialogue_id, int serial_number, DateTime time, IEnumerable<ResponseOption> options)
		: base(dialogue_id, serial_number, time)
	{
		this.options = options.ToList();
	}

	public void SetChoice(int id, bool notify = true)
	{
		chosenReplyID = id;
		base.State |= MessageState.Delivered;
		if (notify)
		{
			onUpdate.OnNext(this);
		}
	}

	public override GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public override void LoadState(GreenT.Data.Memento memento)
	{
		try
		{
			Memento memento2 = memento as Memento;
			base.LoadState(memento);
			if (memento2 != null && memento2.ChosenReplyID != -1)
			{
				SetChoice(memento2.ChosenReplyID, notify: false);
			}
			else if (base.State != 0)
			{
				SetChoice(0, notify: false);
			}
		}
		catch (InvalidCastException innerException)
		{
			throw new Exception("Exception on trying to load memento \"" + memento.UniqueKey + "\" inside \"" + ToString() + "\"\n memento of type \"" + memento.GetType().ToString() + "\" can't be cast to type \"" + typeof(Memento).ToString() + "\"", innerException);
		}
		catch (Exception innerException2)
		{
			throw new Exception("Exception on trying to load memento \"" + memento.UniqueKey + "\"  inside " + ToString(), innerException2);
		}
	}

	protected override void OnAddFlag(MessageState state)
	{
		if (state.Contains(MessageState.Passed))
		{
			base.State |= MessageState.Read;
		}
	}
}
