using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsSqlSmo
{
	enum DbObjType
	{
		Default,
		Uddt,
		Udtt,
		//
		Table,
		View,
		//
		Udf,
		Sp,
		//
		Ftc,
	}
}
