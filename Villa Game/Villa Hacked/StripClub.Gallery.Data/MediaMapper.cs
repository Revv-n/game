using System;
using GreenT.HornyScapes.Data;
using GreenT.Model.Collections;

namespace StripClub.Gallery.Data;

[Serializable]
[Mapper]
public class MediaMapper
{
	public class Manager : SimpleManager<MediaMapper>
	{
	}

	public int id;

	public int[] tag_id;

	public int[] char_id;

	public string type;

	public MediaMapper(int id, int[] tag_id, int[] char_id, string type)
	{
		this.id = id;
		this.tag_id = tag_id;
		this.char_id = char_id;
		this.type = type;
	}

	public override string ToString()
	{
		return "<Media ID: " + id + "; Type: " + type + ">";
	}
}
