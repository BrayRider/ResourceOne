using System;

namespace RSM.Web.Library
{
	public class Utilities
	{
		public static byte[] HexToByte(string hexString)
		{
			var returnBytes = new byte[hexString.Length / 2];

			for (var i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

			return returnBytes;
		}
	}
}
