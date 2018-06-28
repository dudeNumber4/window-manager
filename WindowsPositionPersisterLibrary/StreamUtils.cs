using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowPositionPersisterLibrary
{

	internal class StreamUtils
	{

		internal static byte[] StreamToBytes(Stream stream)
		{
			Debug.Assert(stream != null);
			stream.Position = 0;
			byte[] buffer = new byte[stream.Length];
			int read = 0;

			int chunk;
			while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
			{
				read += chunk;

				// If we've reached the end of our buffer, check to see if there's any more information (may be with a network stream)
				if (read == buffer.Length)
				{
					int nextByte = stream.ReadByte();

					// End of stream? If so, we're done
					if (nextByte == -1)
					{
						return buffer;
					}

					// Nope. Resize the buffer, put in the byte we've just read, and continue
					byte[] newBuffer = new byte[buffer.Length * 2];
					Array.Copy(buffer, newBuffer, buffer.Length);
					newBuffer[read] = (byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}

			// Buffer is now too big. Shrink it.
			var result = new byte[read];
			Array.Copy(buffer, result, read);
			return result;
		}

	}

}