using System;

namespace StripClub.Model;

public interface IUserInfo
{
	int UID { get; }

	DateTime FirstLogin { get; }

	DateTime LastLogin { get; }
}
