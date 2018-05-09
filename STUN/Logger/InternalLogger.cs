using System;

namespace STUN {
	internal sealed class InternalLogger : ILogger {
		public void Error(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Warn(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Info(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Debug(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void Trace(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		public void TraceVerbose(string message, ConsoleColor FC, ConsoleColor BC) {
			Console.BackgroundColor = BC;
			Console.ForegroundColor = FC;
			Console.WriteLine(message);
			Console.ResetColor();
		}
	}
}
