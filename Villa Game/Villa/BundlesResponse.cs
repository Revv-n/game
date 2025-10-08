using System;
using System.Collections.Generic;

[Serializable]
public class BundlesResponse
{
	public string status;

	public string error;

	public List<BundleData> data;
}
