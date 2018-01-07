using STUN.Utils;

namespace STUN.me.stojan.stun.message.attribute {
	public interface IAttr {
		void WriteToBuffer(ref ByteBuffer buffer);
	}
}
