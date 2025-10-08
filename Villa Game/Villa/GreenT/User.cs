using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Net;
using UniRx;
using UnityEngine;

namespace GreenT;

public class User : IEquatable<User>, IDisposable
{
	[Flags]
	public enum State
	{
		Unknown = 0,
		Logged = 1,
		Registered = 4
	}

	public static readonly User Unknown = new User();

	private Subject<User> onUpdate;

	private Subject<User> onLogin;

	private string playerID;

	private string platformId;

	private const string defaultName = "Boss";

	private DateTime lastUpdate;

	private string nickname;

	private string fb_id;

	private string apple_id;

	private Regex checkForHexRegExp = new Regex("^[0-9a-fA-F]{24}$");

	public IObservable<User> OnUpdate => onUpdate;

	public ISubject<User> OnLogin => onLogin;

	public string PlayerID => playerID;

	public string PlaftormId => platformId;

	public string Nickname
	{
		get
		{
			return nickname;
		}
		set
		{
			nickname = value;
			onUpdate.OnNext(this);
		}
	}

	public DateTime Updated => lastUpdate;

	public string FBID => fb_id;

	public string AppleID => apple_id;

	public MailAddress Email { get; private set; }

	public State Type { get; private set; }

	public bool IsGuest { get; private set; }

	public string ErolabsNick { get; private set; }

	public User()
	{
		onUpdate = new Subject<User>();
		onLogin = new Subject<User>();
		Nickname = "Boss";
	}

	public IObservable<User> OnAuthorizedUser()
	{
		return OnLogin.Where((User user) => !user.Type.Equals(State.Unknown));
	}

	public void Init()
	{
		SetPlayerData(Unknown.Nickname, Unknown.playerID);
		lastUpdate = Unknown.lastUpdate;
		SetupEmail(string.Empty);
		PlayerPrefs.DeleteKey("Player ID");
		PlayerPrefs.DeleteKey(RequestInstaller.PlayerIdKey);
		PlayerPrefs.Save();
		onUpdate.OnNext(this);
	}

	public void SetPlatformId(string platform_id)
	{
		platformId = platform_id;
	}

	public void Init(UserDataMapper mapper, bool isGuest, string erolabsNick)
	{
		IsGuest = isGuest;
		ErolabsNick = erolabsNick;
		Init(mapper.PlayerID, mapper.Nickname, mapper.EmailAdress, mapper.Updated());
	}

	public void Init(UserDataMapper mapper)
	{
		Init(mapper.PlayerID, mapper.Nickname, mapper.EmailAdress, mapper.Updated());
	}

	public void Init(string player_id, string user_name, string email, DateTime last_update)
	{
		string userName = (string.IsNullOrEmpty(user_name) ? "Boss" : user_name);
		SetPlayerData(userName, player_id);
		lastUpdate = last_update;
		SetupEmail(email);
		SetupType();
		PlayerPrefs.SetString(RequestInstaller.PlayerIdKey, playerID);
		PlayerPrefs.Save();
		onUpdate.OnNext(this);
	}

	public bool IsPlayerValid()
	{
		try
		{
			return checkForHexRegExp.IsMatch(PlayerID);
		}
		catch
		{
			return false;
		}
	}

	public bool Equals(User other)
	{
		if (string.IsNullOrEmpty(playerID) || !playerID.Equals(other.playerID))
		{
			if (string.IsNullOrEmpty(playerID))
			{
				return string.IsNullOrEmpty(other.playerID);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return playerID?.GetHashCode() ?? 0;
	}

	public void LoginNotify()
	{
		onLogin.OnNext(this);
	}

	public string UniqueKey()
	{
		return "User";
	}

	private void SetPlayerData(string userName, string playerId, State type = State.Unknown)
	{
		Nickname = userName;
		playerID = playerId;
		Type = type;
	}

	private void SetupEmail(string email)
	{
		try
		{
			Email = new MailAddress(email);
		}
		catch
		{
			Email = null;
		}
	}

	private void SetupType()
	{
		if (PlatformHelper.IsEpochaMonetization() || PlatformHelper.IsHaremMonetization())
		{
			Type = State.Unknown;
			if (IsPlayerValid())
			{
				Type |= State.Logged;
			}
			if (IsMailValid() || IsFBTokenValid() || IsAppleIDValid())
			{
				Type |= State.Registered;
			}
		}
		else if (PlatformHelper.IsNutakuMonetization() || PlatformHelper.IsSteamMonetization())
		{
			Type = State.Registered;
		}
		else
		{
			if (!PlatformHelper.IsErolabsMonetization())
			{
				throw new NotImplementedException();
			}
			Type = State.Unknown;
			if (IsGuest)
			{
				Type |= State.Logged;
			}
			else
			{
				Type |= State.Registered;
			}
		}
	}

	private bool IsAppleIDValid()
	{
		return !string.IsNullOrEmpty(apple_id);
	}

	private bool IsFBTokenValid()
	{
		return !string.IsNullOrEmpty(fb_id);
	}

	private bool IsMailValid()
	{
		return Email != null;
	}

	public void Dispose()
	{
		onUpdate.Dispose();
		onLogin.Dispose();
	}
}
