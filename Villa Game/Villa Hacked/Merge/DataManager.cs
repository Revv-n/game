using System.Collections.Generic;
using System.IO;
using Merge.Core.Masters;
using UnityEngine;

namespace Merge;

public class DataManager
{
	private const bool ALLOW_LOGS = false;

	public static readonly string SavePath = Application.persistentDataPath + "/Data";

	private List<IDataController> data_controllers;

	public DataManager(IList<BaseController> controllers)
	{
		data_controllers = new List<IDataController>();
		foreach (BaseController controller in controllers)
		{
			if (controller is IDataController)
			{
				data_controllers.Add(controller as IDataController);
			}
		}
	}

	public static void SimpleDeleteData()
	{
		Log("Delete Data Folder");
		if (Directory.Exists(SavePath))
		{
			Directory.Delete(SavePath, recursive: true);
			Log("Data deleted");
		}
		else
		{
			Log("Data already empty");
		}
	}

	private static void Log(string str)
	{
	}
}
