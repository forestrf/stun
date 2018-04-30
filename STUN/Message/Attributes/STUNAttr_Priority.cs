using BBuffer;
using STUN.Message.Enums;

namespace STUN.Message.Attributes {
	public struct STUNAttr_Priority : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.PRIORITY;

		public uint priority;

		public STUNAttr_Priority(uint priority) {
			this.priority = priority;
		}

		public STUNAttr_Priority(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			STUNTypeLengthValue.WriteTypeLength(TYPE, 4, ref buffer);
			buffer.Put(priority);
			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			priority = buffer.GetUInt();
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("Priority: ").Append(priority).Append("\n");
			return s.ToString();
		}
	}
}
