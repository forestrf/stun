using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message.Attributes {
	public struct STUNAttr_IceControlling : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.ICE_CONTROLLING;

		public ulong tieBreaker;

		public STUNAttr_IceControlling(ulong tieBreaker) {
			this.tieBreaker = tieBreaker;
		}

		public STUNAttr_IceControlling(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 8, ref buffer);
			buffer.Put(tieBreaker);
			STUNTypeLengthValue.AddPadding(ref buffer);
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
