using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_Priority : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.PRIORITY;

		public uint priority;

		public STUNAttribute_Priority(uint value) {
			this.priority = value;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, 4, ref buffer);
			buffer.Put(priority);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
