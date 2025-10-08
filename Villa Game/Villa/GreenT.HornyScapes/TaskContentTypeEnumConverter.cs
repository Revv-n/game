using GreenT.Types;

namespace GreenT.HornyScapes;

public sealed class TaskContentTypeEnumConverter : EnumConverterM<TaskContentType, ContentType>
{
	public override ContentType Convert(TaskContentType from)
	{
		return from switch
		{
			TaskContentType.Main => ContentType.Main, 
			TaskContentType.Event => ContentType.Event, 
			_ => ContentType.Main, 
		};
	}
}
