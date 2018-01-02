using STUN.Crypto;
using STUN.Utils;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_MessageIntegrity : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.MESSAGE_INTEGRITY;
		private const ushort HMAC_LENGTH = 20;

		HMAC_SHA1 hmacsha1Instance;

		public STUNAttribute_MessageIntegrity(string key, ref HMAC_SHA1 hmacsha1Instance) {
			if (null == hmacsha1Instance)
				hmacsha1Instance = new HMAC_SHA1(Encoding.UTF8.GetBytes(key));
			this.hmacsha1Instance = hmacsha1Instance;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			int count = buffer.Position;
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, HMAC_LENGTH, ref buffer);
			STUNMessageBuilder.UpdateAttributesLength(ref buffer, buffer.Position + HMAC_LENGTH);
			byte[] hmac = hmacsha1Instance.ComputeHash(buffer.data, buffer.absOffset, count);
			buffer.Put(hmac);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
