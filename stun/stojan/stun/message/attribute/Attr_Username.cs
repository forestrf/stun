using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct Attr_Username : IAttr {
		public const STUNAttribute TYPE = STUNAttribute.USERNAME;

		public ByteBuffer buffer;

		public Attr_Username(string value) {
			buffer = new ByteBuffer(System.Text.Encoding.UTF8.GetBytes(value));
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) this.buffer.Length, ref buffer);
			if (this.buffer.HasData() && this.buffer.Length <= 512)
				buffer.Put(this.buffer);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
