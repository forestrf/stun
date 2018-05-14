using System;

namespace STUN {
	internal sealed class InternalLogger : ILogger {
		public void Error(string message) {
			Console.WriteLine(message);
		}

		public void Warn(string message) {
			Console.WriteLine(message);
		}

		public void Info(string message) {
			Console.WriteLine(message);
		}

		public void Debug(string message) {
			Console.WriteLine(message);
		}

		public void Trace(string message) {
			Console.WriteLine(message);
		}

		public void TraceVerbose(string message) {
			Console.WriteLine(message);
		}
	}
}
