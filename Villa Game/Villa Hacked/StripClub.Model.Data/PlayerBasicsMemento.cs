using System;
using GreenT.Data;

namespace StripClub.Model.Data;

[Serializable]
public class PlayerBasicsMemento : Memento
{
	public int level;

	public ReplyMapper energy;

	public ReplyMapper reply;

	public PlayerBasicsMemento(PlayersData playersData)
		: base(playersData)
	{
		level = playersData.Level.Value;
		energy = new ReplyMapper(playersData.Energy);
		reply = new ReplyMapper(playersData.Replies);
	}
}
