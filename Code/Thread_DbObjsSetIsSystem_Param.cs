using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;
using MsSqlSmo;

namespace DbScripter
{
	class Thread_DbObjsSetIsSystem_Param
	{
		public AutoResetEvent EndEvent;
		public Exception Result;
		public Database Db;
		public DbObjsSameType DbObjs;
		public int DbObjs_BeginIndex;
		public int DbObjs_EndIndex;

		public Thread_DbObjsSetIsSystem_Param(
			AutoResetEvent _EndEvent
			, Exception _Result
			, Database _Db
			, DbObjsSameType _DbObjs
			, int _DbObjs_BeginIndex
			, int _DbObjs_EndIndex
			)
		{
			this.EndEvent = _EndEvent;
			this.Result = _Result;
			this.Db = _Db;
			this.DbObjs = _DbObjs;
			this.DbObjs_BeginIndex = _DbObjs_BeginIndex;
			this.DbObjs_EndIndex = _DbObjs_EndIndex;
		}

	}
}
