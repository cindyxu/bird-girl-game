using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class Log
{
	public const string CUTSCENE = "CUTSCENE";
	public const string AI_SCAN = "AI_SCAN";
	public const string AI_SEARCH = "AI_SEARCH";
	public const string AI_PLAN = "AI_PLAN";
	public const string AI_INPUT = "AI_INPUT";
	public const string DIALOGUE = "DIALOGUE";
	public const string TRIGGER = "TRIGGER";
	public const string LOCOMOTION = "LOCOMOTION";
	public const string ROOM = "ROOM";
	public const string DEMO = "DEMO";

	private static string[] filters;
	public static readonly Logger logger;

	static Log () {
		filters = new string[] {
		};
		logger = new Logger (new LogHandler (filters));
	}

	public class LogHandler : ILogHandler {

		private string[] mFilters;

		public LogHandler (string[] filters) {
			mFilters = filters;
		}

		public void LogFormat (LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			if (Array.IndexOf (mFilters, args [0]) >= 0) {
				Debug.logger.logHandler.LogFormat (logType, context, format, args);
			}
		}

		public void LogException (Exception exception, UnityEngine.Object context)
		{
			Debug.logger.logHandler.LogException (exception, context);
		}
	}
		
}