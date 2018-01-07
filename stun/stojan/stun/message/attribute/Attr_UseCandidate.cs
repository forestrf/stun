using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public struct Attr_UseCandidate : IAttr {
		public const STUNAttribute TYPE = STUNAttribute.USE_CANDIDATE;
		
		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, 0, ref buffer);
			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
