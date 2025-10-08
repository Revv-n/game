using System;
using System.Runtime.Serialization;
using GreenT.Data;

namespace StripClub.Model.Data;

[Serializable]
public class ItemMapper : Memento, IDeserializationCallback
{
	public byte[] guidBytes;

	public Guid Guid { get; private set; }

	public int Amount { get; }

	public ItemMapper(Item item)
		: base(item)
	{
		Guid = item.Info.Guid;
		guidBytes = Guid.ToByteArray();
		Amount = item.Amount.Value;
	}

	public void OnDeserialization(object sender)
	{
		Guid = new Guid(guidBytes);
	}
}
