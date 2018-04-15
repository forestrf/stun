using STUN.Crypto;
using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Fingerprint : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.FINGERPRINT;

		private const int FINGERPRINT_XOR = 0x5354554e;

		public uint crc;

		public STUNAttr_Fingerprint(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			int count = buffer.Position;
			STUNTypeLengthValue.WriteTypeLength(TYPE, 4, ref buffer);
			STUNMessageBuilder.UpdateHeaderAttributesLength(ref buffer, buffer.Position + 4);
			crc = Crc32.CRC32.Calculate(buffer.data, buffer.absOffset, count) ^ FINGERPRINT_XOR;
			buffer.Put(crc);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			crc = buffer.GetUInt() ^ FINGERPRINT_XOR;
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("crc: ").Append(crc).Append("\n");
			return s.ToString();
		}
	}
}
