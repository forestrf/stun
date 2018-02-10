namespace STUN.Message.Enums {
	public static class STUNClassConst {
		public const int Mask = 0x0110;
	}

	public enum STUNClass : int {
		Request = 0x0000,
		Indication = 0x0010,
		Success = 0x0100,
		Error = 0x0110,
	}
}
