using BBuffer;
using STUN.Message.Enums;

namespace STUN.Message.Attributes {
	public struct STUNAttr_UseCandidate : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.USE_CANDIDATE;

		public STUNAttr_UseCandidate(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 0, ref buffer);
			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			return s.ToString();
		}
	}
}
