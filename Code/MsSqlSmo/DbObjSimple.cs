using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MsSqlSmo
{
	class DbObjSimple
	{
		public string Name;
		public bool IsSystem = false;
		public StringCollection Script = null;

		public DbObjSimple(
			string _Name
			)
		{
			this.Name = _Name;
		}

	}
}
