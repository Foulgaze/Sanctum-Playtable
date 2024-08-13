using UnityEngine;

public class Logger
{
	public static void LogError(string message)
	{
		Debug.LogError(message);
	}

	public static void Log(string message)
	{
		Debug.Log(message);
	}
}