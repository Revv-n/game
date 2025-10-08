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
		IObservable<Response<UserDataMapper>> observable = Observable.Share<Response<UserDataMapper>>(HttpRequestExecutor.PostRequest<Response<UserDataMapper>>(requestUrl, fields));
		IObservable<Response<UserDataMapper>> observable2 = Observable.ContinueWith<Notification<Response<UserDataMapper>>, Response<UserDataMapper>>(Observable.Where<Notification<Response<UserDataMapper>>>(Observable.Materialize<Response<UserDataMapper>>(observable), (Func<Notification<Response<UserDataMapper>>, bool>)((Notification<Response<UserDataMapper>> _notification) => _notification.Exception != null && _notification.Exception is UnityWebRequestException ex2 && ex2.ResponseCode == 515)), (Func<Notification<Response<UserDataMapper>>, IObservable<Response<UserDataMapper>>>)delegate
		{
			fields["crypted_password"] = ScryptEncode.Encode(initPassword, fields["email"]);
			return HttpRequestExecutor.PostRequest<Response<UserDataMapper>>(requestUrl, fields);
		});
		return Observable.Merge<Response<UserDataMapper>>(Observable.Dematerialize<Response<UserDataMapper>>(Observable.Where<Notification<Response<UserDataMapper>>>(Observable.Materialize<Response<UserDataMapper>>(observable), (Func<Notification<Response<UserDataMapper>>, bool>)((Notification<Response<UserDataMapper>> _notification) => _notification.Exception == null || !(_notification.Exception is UnityWebRequestException ex) || ex.ResponseCode != 515))), new IObservable<Response<UserDataMapper>>[1] { observable2 });
	}
}
