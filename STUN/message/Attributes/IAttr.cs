using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public interface ISTUNAttribute {
		void WriteToBuffer(ref ByteBuffer buffer);
	}
}
