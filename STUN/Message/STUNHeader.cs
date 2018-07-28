using STUN.Message.Enums;

namespace STUN.Message {
	/// <summary>
	/// Utilities for forming STUN message headers.
	/// </summary>
	public static class STUNHeader {
		/// <summary>
		/// The STUN "magic cookie". Network Byte Order
		/// </summary>
		internal const uint MAGIC_COOKIE = 0x2112A442;
		private const int ClassMask = 0x0110;
		private const int MethodMask = ~ClassMask;

		public static STUNClass GetClass(int messageType) {
			return (STUNClass) (messageType & ClassMask);
		}

		public static STUNMethod GetMethod(int messageType) {
			return (STUNMethod) (messageType & MethodMask);
		}
	}
}
