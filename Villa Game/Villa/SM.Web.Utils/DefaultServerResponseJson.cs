using UnityEngine;

namespace SM.Web.Utils;

public class DefaultServerResponseJson<T>
{
	[SerializeField]
	protected string status;

	[SerializeField]
	private string error;

	[SerializeField]
	protected T data;

	public string Error => error;

	public T Data => data;

	public virtual bool GetStatus(out int status_value)
	{
		return int.TryParse(status, out status_value);
	}

	public bool ValidateStatus()
	{
		if (GetStatus(out var status_value))
		{
			return status_value == 0;
		}
		return false;
	}
}
