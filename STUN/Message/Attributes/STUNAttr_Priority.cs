using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Priority : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.PRIORITY;

		public uint priority;

		public STUNAttr_Priority(uint priority) {
			this.priority = priority;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 4, ref buffer);
			buffer.Put(priority);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			priority = buffer.GetUInt();
		}
	}
}
