using System;
using UnityEngine;

namespace Games.Coresdk.Unity;

public class ProfileResult
{
	[Serializable]
	public class ProfileData
	{
		public string result;

		public string server_time;

		public user_info user_info;
	}

	[Serializable]
	public class user_info
	{
		public string birthday;

		public string country;

		public string free_coins;

		public string gender;

		public string user_id;

		public string hobbies;

		public string coins;

		public string nickname;

		public string account_status;

		public string account;
	}

	public Exception Exception { get; private set; }

	public ProfileData Data { get; private set; }

	public bool IsSuccess { get; private set; }

	public static ProfileResult Parse(RawResponse rawResponse)
	{
		ProfileResult profileResult = new ProfileResult();
		if (rawResponse.Exception != null)
		{
			profileResult.Exception = rawResponse.Exception;
			return profileResult;
		}
		ProfileData profileData2 = (profileResult.Data = JsonUtility.FromJson<ProfileData>(rawResponse.Data));
		profileResult.IsSuccess = profileResult.Exception == null && profileData2 != null && profileData2.result != null && profileData2.result == "0000";
		return profileResult;
	}
}
