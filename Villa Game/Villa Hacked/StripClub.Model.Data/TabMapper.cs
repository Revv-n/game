using System;

namespace StripClub.Model.Data;

[Serializable]
public class TabMapper
{
	public int id;

	public int position;

	public string icon;

	public UnlockType unlock_type;

	public int unlock_value;
}
