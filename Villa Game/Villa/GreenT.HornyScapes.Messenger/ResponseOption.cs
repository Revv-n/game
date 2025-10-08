using System.Collections.Generic;
using GreenT.Types;

namespace GreenT.HornyScapes.Messenger;

public class ResponseOption
{
	public CompositeIdentificator DialogueSerialID;

	public int ID { get; private set; }

	public string LocalizationKey { get; }

	public IEnumerable<IItemLot> NecessaryItems { get; }

	public ResponseOption(int dialogueID, int serialNumber, int id, IEnumerable<IItemLot> necessaryItems)
	{
		ID = id;
		DialogueSerialID = new CompositeIdentificator(dialogueID, serialNumber);
		LocalizationKey = "content.chat." + dialogueID + "." + serialNumber + ((id == 0) ? ".left" : ".right");
		NecessaryItems = necessaryItems;
	}
}
