using BBuffer;
using STUN.Message.Enums;

namespace STUN.Message.Attributes {
	public struct STUNAttr : ISTUNAttr {
		public readonly STUNAttribute type;
		public readonly ByteBuffer data;
		public readonly ByteBuffer stunMessage;
		public readonly bool isValid;

		public STUNAttr(STUNAttribute type, ByteBuffer data, ByteBuffer stunMessage) {
			this.type = type;
			this.data = data;
			this.stunMessage = stunMessage;
			isValid = true;
		}

		public STUNAttr(ref ByteBuffer buffer) {
			ushort attrLength;
			isValid = STUNTypeLengthValue.ReadTypeLength(ref buffer, out type, out attrLength);
			data = new ByteBuffer(buffer.data, buffer.absPosition, attrLength);
			stunMessage = new ByteBuffer(buffer.data, buffer.absOffset, buffer.Length);
			buffer.Position += attrLength;
			buffer.Pad4();
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(type, (ushort) data.Length, ref buffer);
			if (data.HasData()) buffer.Put(data);
			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			// Not intended to be used this way
			this = attr;
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(type).Append("\n");

			if (isValid) {
				for (int i = 0; i < data.Length; i++) {
					s.Append(data[i].ToString("X2"));
					if (i < data.Length - 1) s.Append(":");
				}
				s.Append("\n");
			}
			else {
				s.Append("Invalid attribute (length too large)").Append("\n");
			}

			return s.ToString();
		}
	}
}
