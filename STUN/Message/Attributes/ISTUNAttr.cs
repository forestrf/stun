using BBuffer;

namespace STUN.Message.Attributes {
	public interface ISTUNAttr {
		void WriteToBuffer(ref ByteBuffer buffer);
		void ReadFromBuffer(STUNAttr attr);
	}
}
