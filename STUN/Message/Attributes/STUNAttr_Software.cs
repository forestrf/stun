using STUN.Message.Enums;
using STUN.NetBuffer;
using System;
using System.Text;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Software : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.SOFTWARE;

		public string software;

		public STUNAttr_Software(string software) {
			this.software = software;
		}

		public STUNAttr_Software(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ByteBuffer attrStart = buffer; // Make a copy that will retain the current position
			STUNTypeLengthValue.WriteTypeLength(0, 0, ref buffer); // write temporal values

			int length = Encoding.UTF8.GetBytes(software, 0, Math.Min(128, software.Length), buffer.data, buffer.absPosition);
			buffer.absPosition += length;

			STUNTypeLengthValue.WriteTypeLength(TYPE, (ushort) length, ref attrStart); // Write definitive values

			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			software = Encoding.UTF8.GetString(attr.data.data, attr.data.absPosition, Math.Min(128, attr.data.Length));
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append(TYPE).Append("\n");
			s.Append("Software: ").Append(software).Append("\n");
			return s.ToString();
		}
	}
}
