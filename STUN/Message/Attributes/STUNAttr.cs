using BBuffer;
using STUN.Message.Enums;
using System.Collections.Generic;

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
			this = attr;
		}

		private static int AttributeIndexOf(IList<STUNAttr> attributes, STUNAttribute type) {
			for (int i = 0; i < attributes.Count; i++) {
				if (type == attributes[i].type) {
					return i;
				}
			}
			return -1;
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(type).Append("\n");

			for (int i = 0; i < data.Length; i++) {
				s.Append(data[i].ToString("X2"));
				if (i < data.Length - 1) s.Append(":");
			}
			s.Append("\n");

			return s.ToString();
		}
	}
}
