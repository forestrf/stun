using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr : ISTUNAttr {
		public readonly STUNAttribute type;
		public readonly ByteBuffer data;

		public STUNAttr(STUNAttribute type, ByteBuffer data) {
			this.type = type;
			this.data = data;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(type, (ushort) data.Length, ref buffer);
			if (data.HasData()) buffer.Put(data);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}
	}
}
