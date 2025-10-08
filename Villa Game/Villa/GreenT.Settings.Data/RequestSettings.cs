using System.Collections.Generic;
using UnityEngine;

namespace GreenT.Settings.Data;

[CreateAssetMenu(menuName = "GreenT/Connection Settings/Requests/Remote", order = 1)]
public class RequestSettings : ScriptableObject, IRequestUrlResolver
{
	[SerializeField]
	protected string appName;

	[SerializeField]
	protected string ratingAppName;

	public string AppName => appName;

	public string RatingAppName => ratingAppName;

	[field: SerializeField]
	protected UserRequestSettings UserRequestSettings { get; private set; }

	public virtual string PostRequestUrl(PostRequestType type)
	{
		if (UserRequestSettings.Object.TryGetValue(type, out var value))
		{
			return value;
		}
		throw new KeyNotFoundException(type.ToString());
	}
}
