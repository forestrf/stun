using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttr : ISTUNAttr {
		private STUNAttribute type;
		private ByteBuffer data;

		public STUNAttr(STUNAttribute type, ByteBuffer data) {
			this.type = type;
			this.data = data;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) type, (ushort) data.Length, ref buffer);
			buffer.Put(data);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
