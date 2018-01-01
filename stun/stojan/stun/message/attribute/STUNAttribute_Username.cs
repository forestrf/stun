using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_Username : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.USERNAME;

		public ByteBuffer buffer;

		public STUNAttribute_Username(string value) {
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
