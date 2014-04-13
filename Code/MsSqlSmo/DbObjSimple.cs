using System.Collections.Specialized;

namespace MsSqlSmo
{
	class DbObjSimple
	{
		public string Name;
		public string Schema;
		public bool IsSystem = false;
		public StringCollection Script = null;
		public double DataSpaceUsed = 0;
		public double IndexSpaceUsed = 0;

		public DbObjSimple(
			string _Name
			,string _Schema
			)
		{
			this.Name = _Name;
			this.Schema = _Schema;
		}

	}
}
