using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Priority : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.PRIORITY;

		private uint priority;

		public STUNAttr_Priority(uint value) {
			this.priority = value;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 4, ref buffer);
			buffer.Put(priority);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}
	}
}
