using STUN.Crypto;
using STUN.Utils;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_MessageIntegrity : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.MESSAGE_INTEGRITY;

		HMAC_SHA1 hmacsha1Instance;

		public STUNAttribute_MessageIntegrity(string key, ref HMAC_SHA1 hmacsha1Instance) {
			if (hmacsha1Instance == null)
				hmacsha1Instance = new HMAC_SHA1(Encoding.UTF8.GetBytes(key));
			this.hmacsha1Instance = hmacsha1Instance;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			byte[] hmac = hmacsha1Instance.ComputeHash(buffer.data, buffer.absOffset, buffer.Position);
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) hmac.Length, ref buffer);
			buffer.Put(hmac);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
