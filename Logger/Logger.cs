using System.Diagnostics;

namespace STUN {
	public static class Logger {
		private const string KEYWORD = "STUN_";
		public static ILogger logger = new InternalLogger();

		[Conditional(KEYWORD + "TRACEVERBOSE"), Conditional(KEYWORD + "TRACE"), Conditional(KEYWORD + "DEBUG"), Conditional(KEYWORD + "INFO"), Conditional(KEYWORD + "WARN"), Conditional(KEYWORD + "ERROR")]
		public static void Error(string message) {
			if (logger != null) logger.Error(message);
		}

		[Conditional(KEYWORD + "TRACEVERBOSE"), Conditional(KEYWORD + "TRACE"), Conditional(KEYWORD + "DEBUG"), Conditional(KEYWORD + "INFO"), Conditional(KEYWORD + "WARN")]
		public static void Warn(string message) {
			if (logger != null) logger.Warn(message);
		}

		[Conditional(KEYWORD + "TRACEVERBOSE"), Conditional(KEYWORD + "TRACE"), Conditional(KEYWORD + "DEBUG"), Conditional(KEYWORD + "INFO")]
		public static void Info(string message) {
			if (logger != null) logger.Info(message);
		}

		[Conditional(KEYWORD + "TRACEVERBOSE"), Conditional(KEYWORD + "TRACE"), Conditional(KEYWORD + "DEBUG")]
		public static void Debug(string message) {
			if (logger != null) logger.Debug(message);
		}

		[Conditional(KEYWORD + "TRACEVERBOSE"), Conditional(KEYWORD + "TRACE")]
		public static void Trace(string message) {
			if (logger != null) logger.Trace(message);
		}

		[Conditional(KEYWORD + "TRACEVERBOSE")]
		public static void TraceVerbose(string message) {
			if (logger != null) logger.TraceVerbose(message);
		}
	}
}
