using System;
using System.Diagnostics;

namespace STUN {
	public static class Logger {
		public static ILogger logger = new InternalLogger();

		private static object key = new object();

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO"), Conditional("WARN"), Conditional("ERROR")]
		public static void Error(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.Error(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO"), Conditional("WARN")]
		public static void Warn(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.Warn(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO")]
		public static void Info(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.Info(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG")]
		public static void Debug(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.Debug(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE")]
		public static void Trace(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.Trace(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}

		[Conditional("TRACEVERBOSE")]
		public static void TraceVerbose(string message, ConsoleColor FC = (ConsoleColor) (-1), ConsoleColor BC = (ConsoleColor) (-1)) {
			if (logger != null) lock (key) logger.TraceVerbose(message, FC == (ConsoleColor) (-1) ? Console.ForegroundColor : FC, BC == (ConsoleColor) (-1) ? Console.BackgroundColor : BC);
		}
	}
}
