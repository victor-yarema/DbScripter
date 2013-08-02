using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsSqlSmo
{
	class DbObjsSameType
	{
		public DbObjType Type;
		public DbObjSimple[] Items = new DbObjSimple[0];

		public DbObjsSameType(
			DbObjType _Type,
			DbObjSimple[] _Items
			)
		{
			this.Type = _Type;
			this.Items = _Items;
		}

	}
}
