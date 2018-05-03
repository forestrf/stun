using BBuffer;
using STUN.Message.Enums;

namespace STUN.Message.Attributes {
	public struct STUNAttr_IceControlled : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.ICE_CONTROLLED;

		public ulong tieBreaker;

		public STUNAttr_IceControlled(ulong tieBreaker) {
			this.tieBreaker = tieBreaker;
		}

		public STUNAttr_IceControlled(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 8, ref buffer);
			buffer.Put(tieBreaker);
			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			tieBreaker = attr.data.GetULong();
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("TieBreaker: ").Append(tieBreaker).Append("\n");
			return s.ToString();
		}
	}
}
