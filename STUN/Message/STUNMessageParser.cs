using STUN.Message.Attributes;
using STUN.Message.Enums;
using STUN.NetBuffer;
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
		public readonly bool isValid;
		public ByteBuffer buffer;

		public static bool IsSTUN(byte[] buffer, int startPosition) {
			return 0 == (0xFE & buffer[startPosition]);
		}

		/// <summary>
		/// Create a parser from the input stream.
		/// </summary>
		/// <param name="inputStream">The input stream, must not be null</param>
		public STUNMessageParser(ByteBuffer inputStream, List<STUNAttr> attrs) : this() {
			if (null == attrs) return;
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

			if (STUNMessageBuilder.HEADER_LENGTH + length != buffer.Length) {
				return;
			}

			FillAttributesArray(attrs);

			// check CRC if any, and fingerprint. TO DO

			isValid = true;
		}

		public void FillAttributesArray(List<STUNAttr> attributes) {
			buffer.Rewind();
			buffer.SkipBytes(STUNMessageBuilder.HEADER_LENGTH);
			while (buffer.Position < STUNMessageBuilder.HEADER_LENGTH + length) {
				STUNAttribute type;
				ushort length;
				STUNTypeLengthValue.ReadTypeLength(ref buffer, out type, out length);
				STUNAttr attr = new STUNAttr(type, new ByteBuffer(buffer.data, buffer.absPosition, length), new ByteBuffer(buffer.data, buffer.absOffset, buffer.Length));
				buffer.Position += length;
				STUNTypeLengthValue.AddPadding(ref buffer);
				attributes.Add(attr);
			}
		}

		public ByteBuffer GetHeader() {
			return new ByteBuffer(buffer.data, buffer.absOffset, STUNMessageBuilder.HEADER_LENGTH);
		}

		/// <summary>
		/// Get a Human Readable description of the STUN message
		/// </summary>
		public override string ToString() {
			System.Text.StringBuilder s = new System.Text.StringBuilder();

			/*
			public readonly STUNClass stunClass;
			public readonly STUNMethod stunMethod;
			public readonly Transaction transaction;
			*/
			s.Append("Length: ").Append(length).Append("\n");
			s.Append("Class: ").Append(stunClass).Append("\n");
			s.Append("Method: ").Append(stunMethod).Append("\n");
			s.Append("Transaction: ").Append(transaction).Append("\n");

			List<STUNAttr> attrs = new List<STUNAttr>();
			FillAttributesArray(attrs);

			s.Append("Attributes (Count: ").Append(attrs.Count).Append("):\n\n");
			foreach (var attr in attrs) {
				ISTUNAttr parsed = new STUNAttr();
				switch (attr.type) {
					case STUNAttribute.ERROR_CODE: parsed = new STUNAttr_ErrorCode(); break;
					case STUNAttribute.FINGERPRINT: parsed = new STUNAttr_Fingerprint(); break;
					case STUNAttribute.MAPPED_ADDRESS: parsed = new STUNAttr_MappedAddress(); break;
					case STUNAttribute.MESSAGE_INTEGRITY: parsed = new STUNAttr_MessageIntegrity(); break;
					case STUNAttribute.PRIORITY: parsed = new STUNAttr_Priority(); break;
					case STUNAttribute.SOFTWARE: parsed = new STUNAttr_Software(); break;
					case STUNAttribute.USE_CANDIDATE: parsed = new STUNAttr_UseCandidate(); break;
					case STUNAttribute.USERNAME: parsed = new STUNAttr_Username(); break;
					case STUNAttribute.XOR_MAPPED_ADDRESS: parsed = new STUNAttr_XORMappedAddress(); break;
				}
				parsed.ReadFromBuffer(attr);
				s.Append(parsed.ToString()).Append("\n");
			}

			return s.ToString();
		}
	}
}
