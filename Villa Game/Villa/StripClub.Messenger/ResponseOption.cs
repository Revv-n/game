using System.Collections.Generic;
using StripClub.Model;

namespace StripClub.Messenger;

public class ResponseOption
{
	public int ID { get; private set; }

	public string LocalizationKey { get; }

	public IEnumerable<ItemLot> NecessaryItems { get; }

	public ResponseOption(int dialogueID, int serialNumber, int id, IEnumerable<ItemLot> necessaryItems)
	{
		ID = id;
		LocalizationKey = "content.chat." + dialogueID + "." + serialNumber + ((id == 0) ? ".left" : ".right");
		NecessaryItems = necessaryItems;
	}
}
