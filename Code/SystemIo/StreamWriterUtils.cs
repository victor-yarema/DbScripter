using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemIo
{
	static class StreamWriterUtils
	{
		public static void Write_StringCollection(StreamWriter sw, StringCollection sc)
		{
			foreach (string s in sc)
			{
				sw.WriteLine(s);
			}
		}

	}
}
