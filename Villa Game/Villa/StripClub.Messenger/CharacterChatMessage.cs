using System;
using System.Collections.Generic;
using StripClub.Extensions;
using UniRx;

namespace StripClub.Messenger;

public sealed class CharacterChatMessage : ChatMessage
{
	private IDisposable onAddFlag;

	public int? MediaID { get; }

	public int CharacterID { get; }

	public bool NamesVisibility { get; private set; }

	public override MessageType MessageType => MessageType.Character;

	public IEnumerable<int> Attachements
	{
		get
		{
			if (!MediaID.HasValue)
			{
				return new int[0];
			}
			return MediaID.Value.AsEnumerable();
		}
	}

	public CharacterChatMessage(int author_id, int dialogue_id, int serial_number, DateTime time, int? mediaID = null)
		: base(dialogue_id, serial_number, time)
	{
		MediaID = mediaID;
		CharacterID = author_id;
	}

	public void SetNamesVisibility(bool value)
	{
		NamesVisibility = value;
	}

	protected override void OnAddFlag(MessageState state)
	{
		if (!base.State.Contains(MessageState.Passed) && base.State.Contains(MessageState.Delivered) && onAddFlag == null)
		{
			onAddFlag = Observable.Timer(TimeSpan.FromSeconds(1.0)).Take(1).Subscribe(delegate
			{
				base.State |= MessageState.Passed;
				onUpdate.OnNext(this);
			});
		}
	}

	public override void Dispose()
	{
		onAddFlag?.Dispose();
		base.Dispose();
	}
}
