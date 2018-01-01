using STUN.Crypto;
using STUN.Utils;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	public struct STUNAttribute_MessageIntegrity : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.MESSAGE_INTEGRITY;

		byte[] hmac;

		public STUNAttribute_MessageIntegrity(string key, ref ByteBuffer buffer, ref HMACSHA1 hmacsha1Instance) {
			if (hmacsha1Instance == null) {
				hmacsha1Instance = new HMACSHA1(Encoding.UTF8.GetBytes(key));
			}
			hmac = hmacsha1Instance.ComputeHash(buffer.data, buffer.absOffset, buffer.Position);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, (ushort) hmac.Length, ref buffer);
			buffer.Put(hmac);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
