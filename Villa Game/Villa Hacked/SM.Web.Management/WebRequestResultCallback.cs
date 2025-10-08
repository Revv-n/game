using System;

namespace SM.Web.Management;

public delegate void WebRequestResultCallback<T>(T content, bool result, string error = "");
public delegate void WebRequestResultCallback(object content, Type outputType, bool result, string error = "");
