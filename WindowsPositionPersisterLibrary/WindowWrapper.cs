using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WindowPositionPersisterLibrary
{

	[DataContract]
	internal class WindowWrapper
	{

		[DataMember]
		internal IntPtr Hwnd;
		[DataMember]
		internal int X;
		[DataMember]
		internal int Y;
		[DataMember]
		internal int Width;
		[DataMember]
		internal int Height;
		[DataMember]
		internal string Title;

		public override int GetHashCode()
		{
			unchecked  // don't care about overflow
			{
				return Hwnd.ToInt32() + X + Y + Width + Height;
			}
		}

	}

}
