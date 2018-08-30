using System.Diagnostics;

namespace STUN {
	public static class Logger {
		public static ILogger logger = new InternalLogger();

		private static object key = new object();

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO"), Conditional("WARN"), Conditional("ERROR")]
		public static void Error(string message) {
			if (logger != null) lock (key) logger.Error(message);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO"), Conditional("WARN")]
		public static void Warn(string message) {
			if (logger != null) lock (key) logger.Warn(message);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG"), Conditional("INFO")]
		public static void Info(string message) {
			if (logger != null) lock (key) logger.Info(message);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE"), Conditional("DEBUG")]
		public static void Debug(string message) {
			if (logger != null) lock (key) logger.Debug(message);
		}

		[Conditional("TRACEVERBOSE"), Conditional("TRACE")]
		public static void Trace(string message) {
			if (logger != null) lock (key) logger.Trace(message);
		}

		[Conditional("TRACEVERBOSE")]
		public static void TraceVerbose(string message) {
			if (logger != null) lock (key) logger.TraceVerbose(message);
		}
	}
}
