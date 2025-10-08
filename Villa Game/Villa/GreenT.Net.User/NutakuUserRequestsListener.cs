using System.Collections.Generic;
using GreenT.Data;
using Zenject;

namespace GreenT.Net.User;

public class NutakuUserRequestsListener : UserRequestsListener
{
	private const string NUTAKU_ID_KEY = "nutaku_id";

	private const string PLAYER_ID_KEY = "player_id";

	public NutakuUserRequestsListener(GreenT.User user, List<UserPostRequestProcessor> userRequestProcessors, [InjectOptional] AuthorizationRequestProcessor authorizationProcessor, RestoreSessionProcessor restoreSessionProcessor, IDataStorage dataStorage)
		: base(user, userRequestProcessors, authorizationProcessor, restoreSessionProcessor, dataStorage)
	{
	}

	protected override void InitUser(UserDataMapper mapper)
	{
		string playerId = GetPlayerId(mapper);
		string nickname = GetNickname();
		string empty = string.Empty;
		user.Init(playerId, nickname, empty, mapper.Updated());
		user.SetPlatformId(GetNutakuId());
	}

	private string GetNickname()
	{
		if (!dataStorage.HasKey("nickname"))
		{
			return string.Empty;
		}
		return dataStorage.GetString("nickname");
	}

	private string GetNutakuId()
	{
		if (dataStorage.HasKey("nutaku_id"))
		{
			return dataStorage.GetString("nutaku_id");
		}
		return "";
	}

	private string GetPlayerId(UserDataMapper mapper)
	{
		if (dataStorage.HasKey("player_id"))
		{
			return dataStorage.GetString("player_id");
		}
		dataStorage.SetString("player_id", mapper.PlayerID);
		return mapper.PlayerID;
	}
}
