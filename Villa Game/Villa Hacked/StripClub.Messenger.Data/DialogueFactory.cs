using Zenject;

namespace StripClub.Messenger.Data;

public class DialogueFactory : IFactory<DialogueConfigMapper, Dialogue>, IFactory
{
	public Dialogue Create(DialogueConfigMapper mapper)
	{
		return new Dialogue(mapper.ID, mapper.ConversationID, mapper.TotalMessages, mapper.TotalPlayerMessages, mapper.TotalMedia);
	}
}
