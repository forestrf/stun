using STUN.Utils;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttr_Username : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.USERNAME;

		private ByteBuffer usernameInBuffer;
		private string usernameInString;

		public STUNAttr_Username(string value) : this() {
			usernameInString = value;
		}

		public STUNAttr_Username(ByteBuffer value) : this() {
			usernameInBuffer = value;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ByteBuffer attrStart = buffer; // Make a copy that will retain the current position
			STUNTypeLengthValue.WriteTypeLength(0, 0, ref buffer); // write temporal values

			int length = 0;
			if (null != usernameInBuffer.data) {
				length = usernameInBuffer.Length;
				buffer.Put(usernameInBuffer);
			} else if (null != usernameInString) {
				length = Encoding.UTF8.GetBytes(usernameInString, 0, usernameInString.Length, buffer.data, buffer.absPosition);
			}
			if (length <= 512) {
				buffer.absPosition += length;
			}

			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) length, ref attrStart); // Write definitive values
			
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
