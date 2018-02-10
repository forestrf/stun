using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr_UseCandidate : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.USE_CANDIDATE;
		
		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 0, ref buffer);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}
	}
}
