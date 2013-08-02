using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using MsSqlSmo;

namespace DbScripter
{
	static class Thread_DbObjsSetIsSystem
	{
		public static void ThreadMain(object ParameterObject)
		{
			Thread_DbObjsSetIsSystem_Param Parameter = (Thread_DbObjsSetIsSystem_Param)ParameterObject;
			Parameter.Result = null;
			try
			{
				ScriptingOptions so = new ScriptingOptions();
				so.Add(ScriptOption.DriAll);
				//so.Add(ScriptOption.Permissions);
				//so.Add(ScriptOption.IncludeIfNotExists);
				if (Parameter.DbObjs.Type == DbObjType.Table)
				{
					so.Add(ScriptOption.Triggers);
					so.Add(ScriptOption.Bindings);
					so.Add(ScriptOption.ClusteredIndexes);
					//so.Add(ScriptOption.WithDependencies);
					so.Add(ScriptOption.ExtendedProperties);
					so.Add(ScriptOption.FullTextIndexes);
				}

				for (int i = Parameter.DbObjs_BeginIndex; i < Parameter.DbObjs_EndIndex; i++)
				{
					switch (Parameter.DbObjs.Type)
					{
						case DbObjType.Default:
							{
								Default DbObj = (Default)Parameter.Db.Defaults[Parameter.DbObjs.Items[i].Name];
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						case DbObjType.Uddt:
							{
								UserDefinedDataType DbObj = (UserDefinedDataType)Parameter.Db.UserDefinedDataTypes[Parameter.DbObjs.Items[i].Name];
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						case DbObjType.Udtt:
							{
								UserDefinedTableType DbObj = (UserDefinedTableType)Parameter.Db.UserDefinedTableTypes[Parameter.DbObjs.Items[i].Name];
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						//
						case DbObjType.Table:
							{
								Table DbObj = (Table)Parameter.Db.Tables[Parameter.DbObjs.Items[i].Name];
								if (DbObj == null || DbObj.IsSystemObject)
								{
									//throw new Exception("Can't find DbObj with name \"" + Parameter.DbObjs.Items[i].Name + "\".");
									Parameter.DbObjs.Items[i].IsSystem = true;
									break;
								}
								Parameter.DbObjs.Items[i].IsSystem = DbObj.IsSystemObject;
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);

								Parameter.DbObjs.Items[i].DataSpaceUsed = DbObj.DataSpaceUsed;
							} break;
						case DbObjType.View:
							{
								View DbObj = (View)Parameter.Db.Views[Parameter.DbObjs.Items[i].Name];
								if (DbObj == null || DbObj.IsSystemObject)
								{
									//throw new Exception("Can't find DbObj with name \"" + Parameter.DbObjs.Items[i].Name + "\".");
									Parameter.DbObjs.Items[i].IsSystem = true;
									break;
								}
								Parameter.DbObjs.Items[i].IsSystem = DbObj.IsSystemObject;
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						//
						case DbObjType.Udf:
							{
								UserDefinedFunction DbObj = (UserDefinedFunction)Parameter.Db.UserDefinedFunctions[Parameter.DbObjs.Items[i].Name];
								if (DbObj == null || DbObj.IsSystemObject)
								{
									//throw new Exception("Can't find DbObj with name \"" + Parameter.DbObjs.Items[i].Name + "\".");
									Parameter.DbObjs.Items[i].IsSystem = true;
									break;
								}
								Parameter.DbObjs.Items[i].IsSystem = DbObj.IsSystemObject;
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						case DbObjType.Sp:
							{
								StoredProcedure DbObj = (StoredProcedure)Parameter.Db.StoredProcedures[Parameter.DbObjs.Items[i].Name];
								if (DbObj == null || DbObj.IsSystemObject)
								{
									//throw new Exception("Can't find DbObj with name \"" + Parameter.DbObjs.Items[i].Name + "\".");
									Parameter.DbObjs.Items[i].IsSystem = true;
									break;
								}
								Parameter.DbObjs.Items[i].IsSystem = DbObj.IsSystemObject;
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						//
						case DbObjType.Ftc:
							{
								FullTextCatalog DbObj = (FullTextCatalog)Parameter.Db.FullTextCatalogs[Parameter.DbObjs.Items[i].Name];
								Parameter.DbObjs.Items[i].Script = DbObj.Script(so);
							} break;
						default:
							throw new Exception("Unknown Type = \"" + Parameter.DbObjs.Type.ToString() + "\".");
					}
				}
			}
			catch (Exception ex)
			{
				Parameter.Result = ex;
			}
			Parameter.EndEvent.Set();
		}
	}
}
