using STUN.Crypto;
using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttr_Fingerprint : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.FINGERPRINT;

		private const int FINGERPRINT_XOR = 0x5354554e;
		
		public void WriteToBuffer(ref ByteBuffer buffer) {
			int count = buffer.Position;
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, 4, ref buffer);
			STUNMessageBuilder.UpdateAttributesLength(ref buffer, buffer.Position + 4);
			uint crc = Crc32.CRC32.Calculate(buffer.data, buffer.absOffset, count) ^ FINGERPRINT_XOR;
			buffer.Put(crc);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
