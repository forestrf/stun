using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr : ISTUNAttr {
		public readonly STUNAttribute type;
		public readonly ByteBuffer data;
		public readonly ByteBuffer stunMessage;

		public STUNAttr(STUNAttribute type, ByteBuffer data, ByteBuffer stunMessage) {
			this.type = type;
			this.data = data;
			this.stunMessage = stunMessage;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(type, (ushort) data.Length, ref buffer);
			if (data.HasData()) buffer.Put(data);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}
		
		public void ReadFromBuffer(STUNAttr attr) {
			// Not intended to be used this way
			throw new System.NotImplementedException();
		}
	}
}
