using STUN.Utils;
using System;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct Attr_Software : IAttr {
		public const STUNAttribute TYPE = STUNAttribute.SOFTWARE;

		private string software;

		public Attr_Software(string software) {
			this.software = software;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ByteBuffer attrStart = buffer; // Make a copy that will retain the current position
			STUNTypeLengthValue.WriteTypeLength(0, 0, ref buffer); // write temporal values
			
			int length = Encoding.UTF8.GetBytes(software, 0, Math.Min(128, software.Length), buffer.data, buffer.absPosition);
			buffer.absPosition += length;

			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) length, ref attrStart); // Write definitive values

			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
