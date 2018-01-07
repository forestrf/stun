using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public interface ISTUNAttr {
		void WriteToBuffer(ref ByteBuffer buffer);
	}
}
