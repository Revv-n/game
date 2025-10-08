using System;
using Newtonsoft.Json;

namespace GreenT;

[Serializable]
public class UserDataMapper
{
	[JsonProperty]
	private string player_id;

	[JsonProperty]
	private string nutaku_id;

	[JsonProperty]
	private string steam_id;

	[JsonProperty]
	private string fb_id;

	[JsonProperty]
	private string apple_id;

	[JsonProperty]
	private string email;

	[JsonProperty]
	private string user_name;

	[JsonProperty]
	private string data;

	[JsonProperty]
	private long updated_at;

	public string PlayerID => player_id;

	public string NutakuID => nutaku_id;

	public string SteamID => steam_id;

	public string Nickname => user_name;

	public string Data => data;

	public string FBID => fb_id;

	public string AppleID => apple_id;

	public string EmailAdress => email;

	public long UpdatedAt => updated_at;

	public DateTime Updated()
	{
		return DateTimeOffset.FromUnixTimeSeconds(updated_at).DateTime;
	}

	public override string ToString()
	{
		return base.ToString() + " ID: " + player_id + " Nick: " + user_name + " Email: " + email + " Updated at:" + Updated().ToString();
	}
}
