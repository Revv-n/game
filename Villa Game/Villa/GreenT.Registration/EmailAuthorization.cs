using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GreenT.Net;
using GreenT.Utilities;
using UniRx;

namespace GreenT.Registration;

public class EmailAuthorization : IPostRequest<Response<UserDataMapper>>, IPostRequest<Response<UserDataMapper>, IDictionary<string, string>>
{
	private readonly string requestUrl;

	public EmailAuthorization(string authorizationUrl)
	{
		requestUrl = authorizationUrl;
	}

	public IObservable<Response<UserDataMapper>> Post(IDictionary<string, string> fields)
	{
		if (!fields.ContainsKey("email"))
		{
			throw new ArgumentNullException("Field \"mail\" must be set");
		}
		if (!fields.ContainsKey("password"))
		{
			throw new ArgumentNullException("Field \"password\" must be set");
		}
		string salt = fields["email_by_pass"];
		string initPassword = fields["password"];
		fields["crypted_password"] = ScryptEncode.Encode(initPassword, salt);
		IObservable<Response<UserDataMapper>> source = HttpRequestExecutor.PostRequest<Response<UserDataMapper>>(requestUrl, fields).Share();
		IObservable<Response<UserDataMapper>> observable = (from _notification in source.Materialize()
			where _notification.Exception != null && _notification.Exception is UnityWebRequestException ex2 && ex2.ResponseCode == 515
			select _notification).ContinueWith(delegate
		{
			fields["crypted_password"] = ScryptEncode.Encode(initPassword, fields["email"]);
			return HttpRequestExecutor.PostRequest<Response<UserDataMapper>>(requestUrl, fields);
		});
		return (from _notification in source.Materialize()
			where _notification.Exception == null || !(_notification.Exception is UnityWebRequestException ex) || ex.ResponseCode != 515
			select _notification).Dematerialize().Merge(observable);
	}
}
