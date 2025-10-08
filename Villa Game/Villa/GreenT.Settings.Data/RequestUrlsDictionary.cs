using System;

namespace GreenT.Settings.Data;

[Serializable]
public class RequestUrlsDictionary : SerializableDictionary<RequestType, string>
{
}
