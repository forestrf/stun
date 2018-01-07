using STUN.Utils;
using System;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttr_ErrorCode : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.ERROR_CODE;
		
		private ushort code;
		private string reason;

		public STUNAttr_ErrorCode(ushort code, string reason) {
			this.code = code;
			this.reason = reason;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ByteBuffer attrStart = buffer; // Make a copy that will retain the current position
			STUNTypeLengthValue.WriteTypeLength(0, 0, ref buffer); // write temporal values
			

			int errorClass = code / 100;
			int errorNumber = code % 100;

			// 4 bytes
			buffer.Put((byte) 0);
			buffer.Put((byte) 0);
			buffer.Put((byte) (errorClass & 0b111));
			buffer.Put((byte) errorNumber);

			int length = Encoding.UTF8.GetBytes(reason, 0, Math.Min(reason.Length, 128), buffer.data, buffer.absPosition);
			buffer.Position += length;

			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) (4 + length), ref attrStart); // Write definitive values

			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
