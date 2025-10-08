using System;

namespace StripClub.Model.Data;

[Serializable]
public class PlayerBasicsMapper
{
	public int level;

	public ReplyMapper reply;

	public Currencies balance;

	public DateTime lastTimeOnline;

	public PlayerBasicsMapper(IPlayerBasics playerBasics)
	{
		balance = playerBasics.Balance;
		level = playerBasics.Level.Value;
		reply = new ReplyMapper(playerBasics.Energy);
		lastTimeOnline = DateTime.Now;
	}
}
