using STUN.Message.Attributes;
using STUN.Message.Enums;
using STUN.NetBuffer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace STUN.Message {
	/// <summary>
	/// Parses STUN messages.
	/// </summary>
	public struct STUNMessageParser {
		public readonly STUNClass stunClass;
		public readonly STUNMethod stunMethod;
		public readonly ushort length;
		public readonly Transaction transaction;
		public readonly bool valid;
		public readonly ByteBuffer buffer;

		/// <summary>
		/// Create a parser from the input stream.
		/// </summary>
		/// <param name="inputStream">The input stream, must not be null</param>
		public STUNMessageParser(ByteBuffer inputStream, ref List<STUNAttr> attrs) : this() {
			if (inputStream.Length < STUNMessageBuilder.HEADER_LENGTH) {
				Logger.Error("The message is not long enough");
				return;
			}
			buffer = inputStream;

			ushort messageType = buffer.GetUShort();
			stunClass = (STUNClass) (STUNClassConst.Mask & messageType);
			stunMethod = (STUNMethod) (STUNMethodConst.Mask & messageType);

			length = buffer.GetUShort();
			if (0 != length % 4) {
				Logger.Warn("STUN header reports a length that is not a multiple of 4");
				return;
			}

			uint magickCookie = buffer.GetUInt();
			if (STUNHeader.MAGIC_COOKIE != magickCookie) {
				Logger.Warn("Wrong Magic Cookie");
				return;
			}

			transaction = new Transaction(new ByteBuffer(buffer.data, buffer.absPosition));
			buffer.Position += transaction.Length;

			// check CRC if any, and fingerprint
			if (null == attrs) attrs = new List<STUNAttr>();

			// inputStream.Length

			if (STUNMessageBuilder.HEADER_LENGTH + length != buffer.Length) {
				return;
			}

			while (buffer.Position < STUNMessageBuilder.HEADER_LENGTH + length) {
				STUNAttribute type;
				ushort length;
				STUNTypeLengthValue.ReadTypeLength(ref buffer, out type, out length);
				STUNAttr attr = new STUNAttr(type, new ByteBuffer(buffer.data, buffer.absPosition, length));
				buffer.Position += length;
				STUNTypeLengthValue.AddPadding(ref buffer);
				attrs.Add(attr);
			}

			valid = true;
		}

		public ByteBuffer GetHeader() {
			return new ByteBuffer(buffer.data, buffer.absOffset, STUNMessageBuilder.HEADER_LENGTH);
		}
	}
}
