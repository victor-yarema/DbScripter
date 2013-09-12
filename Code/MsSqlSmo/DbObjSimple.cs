using System.Collections.Specialized;

namespace MsSqlSmo
{
	class DbObjSimple
	{
		public string Name;
		public bool IsSystem = false;
		public StringCollection Script = null;
		public double DataSpaceUsed = 0;
		public double IndexSpaceUsed = 0;

		public DbObjSimple(
			string _Name
			)
		{
			this.Name = _Name;
		}

	}
}
