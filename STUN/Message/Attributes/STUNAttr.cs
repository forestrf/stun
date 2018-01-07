using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr : ISTUNAttr {
		private STUNAttribute type;
		private ByteBuffer data;

		public STUNAttr(STUNAttribute type, ByteBuffer data) {
			this.type = type;
			this.data = data;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) type, (ushort) data.Length, ref buffer);
			if (data.HasData()) buffer.Put(data);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
