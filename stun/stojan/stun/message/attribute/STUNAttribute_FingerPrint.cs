using STUN.Crypto;
using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_Fingerprint : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.FINGERPRINT;

		private const int FINGERPRINT_XOR = 0x5354554e;
		
		public void WriteToBuffer(ref ByteBuffer buffer) {
			uint crc = Crc32.CRC32.Calculate(buffer.data, buffer.absOffset, buffer.Position) ^ FINGERPRINT_XOR;

			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, 4, ref buffer);
			buffer.Put(crc);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
