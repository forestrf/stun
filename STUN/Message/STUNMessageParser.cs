using BBuffer;
using STUN.Message.Attributes;
using STUN.Message.Enums;
using System.Collections.Generic;

namespace STUN.Message {
	/// <summary>
	/// Parses STUN messages.
	/// </summary>
	public struct STUNMessageParser {
		public readonly STUNClass stunClass;
		public readonly STUNMethod stunMethod;
		public readonly ushort length;
		public Transaction transaction; // Not readonly to allow passing it by ref, but changing it does not change the internal buffer
		public readonly bool isValid;
		public ByteBuffer buffer;

		/// <summary>
		/// Create a parser from the input stream.
		/// </summary>
		/// <param name="inputStream">The input stream, must not be null</param>
		public STUNMessageParser(ByteBuffer inputStream, List<STUNAttr> attrs = null) : this() {
			if (inputStream.Length < STUNMessageBuilder.HEADER_LENGTH) {
				Logger.TraceVerbose("The message is not long enough");
				return;
			}
			buffer = inputStream;

			ushort messageType = buffer.GetUShort();
			stunClass = (STUNClass) (STUNClassConst.Mask & messageType);
			stunMethod = (STUNMethod) (STUNMethodConst.Mask & messageType);

			length = buffer.GetUShort();
			if (0 != length % 4 || STUNMessageBuilder.HEADER_LENGTH + length != buffer.Length) {
				Logger.TraceVerbose("STUN header reports a wrong length");
				return;
			}

			uint magickCookie = buffer.GetUInt();
			if (STUNHeader.MAGIC_COOKIE != magickCookie) {
				Logger.Trace("Wrong Magic Cookie");
				return;
			}

			transaction = new Transaction(new ByteBuffer(buffer.data, buffer.absPosition));
			buffer.Position += transaction.Length;

			if (null != attrs)
				FillAttributesArray(attrs);

			isValid = true;
		}

		public void FillAttributesArray(List<STUNAttr> attributes) {
			attributes.Clear();
			buffer.Position = STUNMessageBuilder.HEADER_LENGTH;
			while (buffer.Position < STUNMessageBuilder.HEADER_LENGTH + length) {
				var attr = new STUNAttr(ref buffer);
				if (attr.isValid) {
					attributes.Add(attr);
				}
				else {
					return;
				}
			}
		}

		public ByteBuffer GetHeader() {
			return new ByteBuffer(buffer.data, buffer.absOffset, STUNMessageBuilder.HEADER_LENGTH);
		}

		public static int AttributeIndexOf(List<STUNAttr> attrs, STUNAttribute type, int startIndex = 0) {
			for (int i = startIndex; i < attrs.Count; i++) {
				if (type == attrs[i].type) {
					return i;
				}
			}
			return -1;
		}

		public static int AttributeLastIndexOf(List<STUNAttr> attrs, STUNAttribute type, int startIndex = 0) {
			for (int i = attrs.Count - 1 - startIndex; i >= 0; i--) {
				if (type == attrs[i].type) {
					return i;
				}
			}
			return -1;
		}

		public static bool TryParse(ByteBuffer buffer, out STUNMessageParser parsed) {
			if (0 == (0xFE & buffer[0])) {
				parsed = new STUNMessageParser(buffer);
				return parsed.isValid;
			}
			parsed = default(STUNMessageParser);
			return false;
		}

		/// <summary>
		/// Get a Human Readable description of the STUN message
		/// </summary>
		public override string ToString() {
			System.Text.StringBuilder s = new System.Text.StringBuilder();

			s.Append("Length: ").Append(length).Append("\n");
			s.Append("Class: ").Append(stunClass).Append("\n");
			s.Append("Method: ").Append(stunMethod).Append("\n");
			s.Append("Transaction: ").Append(transaction.ToString()).Append("\n");

			List<STUNAttr> attrs = new List<STUNAttr>();
			FillAttributesArray(attrs);

			s.Append("Attributes (Count: ").Append(attrs.Count).Append("):\n\n");
			foreach (var attr in attrs) {
				ISTUNAttr parsed = null;
				switch (attr.type) {
					case STUNAttribute.ERROR_CODE: parsed = new STUNAttr_ErrorCode(); break;
					case STUNAttribute.FINGERPRINT: parsed = new STUNAttr_Fingerprint(); break;
					case STUNAttribute.MAPPED_ADDRESS: parsed = new STUNAttr_MappedAddress(); break;
					case STUNAttribute.MESSAGE_INTEGRITY: parsed = new STUNAttr_MessageIntegrity(); break;
					case STUNAttribute.PRIORITY: parsed = new STUNAttr_Priority(); break;
					case STUNAttribute.SOFTWARE: parsed = new STUNAttr_Software(); break;
					case STUNAttribute.USE_CANDIDATE: parsed = new STUNAttr_UseCandidate(); break;
					case STUNAttribute.ICE_CONTROLLING: parsed = new STUNAttr_IceControlling(); break;
					case STUNAttribute.ICE_CONTROLLED: parsed = new STUNAttr_IceControlled(); break;
					case STUNAttribute.USERNAME: parsed = new STUNAttr_Username(); break;
					case STUNAttribute.XOR_MAPPED_ADDRESS: parsed = new STUNAttr_XORMappedAddress(); break;
					default: parsed = attr; break;
				}
				parsed.ReadFromBuffer(attr);

				s.Append(parsed.ToString()).Append("\n");
			}

			return s.ToString();
		}
	}
}
