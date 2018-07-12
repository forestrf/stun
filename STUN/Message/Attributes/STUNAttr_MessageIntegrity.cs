using BBuffer;
using HashUtils;
using STUN.Message.Enums;

namespace STUN.Message.Attributes {
	public struct STUNAttr_MessageIntegrity : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.MESSAGE_INTEGRITY;
		private const ushort HMAC_LENGTH = 20;

		private string key;

		public STUNAttr_MessageIntegrity(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public STUNAttr_MessageIntegrity(string key) {
			this.key = null != key ? key : "";
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			int count = buffer.Position;
			STUNTypeLengthValue.WriteTypeLength(TYPE, HMAC_LENGTH, ref buffer);
			STUNMessageBuilder.UpdateHeaderAttributesLength(ref buffer, buffer.Position + HMAC_LENGTH);
			HMAC_SHA1.ComputeHmacSha1(key, new ByteBuffer(buffer.data, buffer.absOffset, count), buffer.data, buffer.absPosition);
			buffer.Position += HMAC_LENGTH;
			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			// TO DO
			Logger.Warn("ReadFromBuffer is not implemented in STUNAttr_MessageIntegrity");
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			return s.ToString();
		}
	}
}
