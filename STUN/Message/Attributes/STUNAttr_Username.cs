using BBuffer;
using STUN.Message.Enums;
using System;
using System.Text;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Username : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.USERNAME;

		private ByteBuffer usernameInBuffer;
		public string usernameInString;

		public STUNAttr_Username(string value) : this() {
			usernameInString = value;
		}

		public STUNAttr_Username(ByteBuffer value) : this() {
			usernameInBuffer = value;
		}

		public STUNAttr_Username(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ByteBuffer attrStart = buffer; // Make a copy that will retain the current position
			STUNTypeLengthValue.WriteTypeLength(0, 0, ref buffer); // write temporal values

			int length = 0;
			if (null != usernameInBuffer.data) {
				length = usernameInBuffer.Length;
				buffer.Put(usernameInBuffer);
			}
			else if (null != usernameInString) {
				length = Encoding.UTF8.GetBytes(usernameInString, 0, usernameInString.Length, buffer.data, buffer.absPosition);
			}
			if (length <= 512) {
				buffer.absPosition += length;
			}

			STUNTypeLengthValue.WriteTypeLength(TYPE, (ushort) length, ref attrStart); // Write definitive values

			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			usernameInString = Encoding.UTF8.GetString(attr.data.data, attr.data.absPosition, Math.Min(512, attr.data.Length));
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("Username: ").Append(usernameInString).Append("\n");
			return s.ToString();
		}
	}
}
