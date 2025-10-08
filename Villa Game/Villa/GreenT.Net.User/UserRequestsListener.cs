using System;
using System.Collections.Generic;
using GreenT.Data;
using Zenject;

namespace GreenT.Net.User;

public class UserRequestsListener : IInitializable, IDisposable
{
	protected GreenT.User user;

	protected List<UserPostRequestProcessor> userRequestProcessors;

	protected AuthorizationRequestProcessor authorizationProcessor;

	protected RestoreSessionProcessor restoreSessionProcessor;

	protected IDataStorage dataStorage;

	public UserRequestsListener(GreenT.User user, List<UserPostRequestProcessor> userRequestProcessors, [InjectOptional] AuthorizationRequestProcessor authorizationProcessor, RestoreSessionProcessor restoreSessionProcessor, IDataStorage dataStorage)
	{
		this.userRequestProcessors = userRequestProcessors;
		this.authorizationProcessor = authorizationProcessor;
		this.restoreSessionProcessor = restoreSessionProcessor;
		this.user = user;
		this.dataStorage = dataStorage;
	}

	public void Initialize()
	{
		foreach (UserPostRequestProcessor userRequestProcessor in userRequestProcessors)
		{
			userRequestProcessor.AddListener(UpdateUser, -1);
		}
		authorizationProcessor?.AddListener(LoginNotification, 1);
		restoreSessionProcessor.AddListener(LoginNotification, 1);
	}

	private void LoginNotification(Response<UserDataMapper> response)
	{
		if (response.Status.Equals(0))
		{
			user.LoginNotify();
		}
	}

	private void UpdateUser(Response<UserDataMapper> response)
	{
		if (!response.Status.Equals(0))
		{
			return;
		}
		try
		{
			UserDataMapper data = response.Data;
			InitUser(data);
		}
		catch (Exception innerException)
		{
			throw innerException.SendException(GetType().Name ?? "");
		}
	}

	protected virtual void InitUser(UserDataMapper mapper)
	{
		user.Init(mapper.PlayerID, mapper.Nickname, mapper.EmailAdress, mapper.Updated());
	}

	public void Dispose()
	{
		foreach (UserPostRequestProcessor userRequestProcessor in userRequestProcessors)
		{
			userRequestProcessor.RemoveListener(UpdateUser);
		}
	}
}
