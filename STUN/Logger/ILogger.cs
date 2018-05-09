using System;

namespace STUN {
	public interface ILogger {
		void Error(string message, ConsoleColor FC, ConsoleColor BC);
		void Warn(string message, ConsoleColor FC, ConsoleColor BC);
		void Info(string message, ConsoleColor FC, ConsoleColor BC);
		void Debug(string message, ConsoleColor FC, ConsoleColor BC);
		void Trace(string message, ConsoleColor FC, ConsoleColor BC);
		void TraceVerbose(string message, ConsoleColor FC, ConsoleColor BC);
	}
}
