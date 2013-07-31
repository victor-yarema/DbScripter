using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;

using Microsoft.SqlServer.Management.Smo;


namespace DBScripter
{
	class Program
	{
		static private DateTime Start;
		static void Main(string[] args)
		{
			string ServerAddress = null;
			string DatabaseName = null;
			string FileNamePrefix = null;

			for (int i = 0; i < args.Length; i++)
			{
				switch ( args[i].ToLower() )
				{
					case "/s":
					{
						if ( ServerAddress != null )
						{
							throw new Exception("Multiple keys for ServerAddress.");
						}
						if ( ! ( i + 1 < args.Length ) )
						{
							throw new Exception("Missing ServerAddress after corresponding key.");
						}
						ServerAddress = args[ i + 1 ];
					}break;
					case "/d":
					{
						if ( DatabaseName != null )
						{
							throw new Exception("Multiple keys for DatabaseName.");
						}
						if ( ! ( i + 1 < args.Length ) )
						{
							throw new Exception("Missing DatabaseName after corresponding key.");
						}
						DatabaseName = args[ i + 1 ];
					}break;
					case "/f":
					{
						if ( FileNamePrefix != null )
						{
							throw new Exception("Multiple keys for FileNamePrefix.");
						}
						if ( ! ( i + 1 < args.Length ) )
						{
							throw new Exception("Missing FileNamePrefix after corresponding key.");
						}
						FileNamePrefix = args[ i + 1 ];
					}break;
				}
			}

			if ( ServerAddress == null )
			{
				throw new Exception("ServerAddress undefined.");
			}
			if ( DatabaseName == null )
			{
				throw new Exception("DatabaseName undefined.");
			}
			if ( FileNamePrefix == null )
			{
				throw new Exception("FileNamePrefix undefined.");
			}


			Server server = new Server(ServerAddress);
			server.ConnectionContext.LoginSecure = true;
			//server.ConnectionContext.Login = "sa";
			//server.ConnectionContext.Password = "q";
			server.ConnectionContext.Connect();

			Database database = server.Databases[DatabaseName];


			Console.Write("Scripting Defaults.");
			ScriptDefaults(database, FileNamePrefix + " 00 Defaults.sql");
			Console.Write(" Ok\n");

			Console.Write("Scripting Data types.");
			ScriptUDTs(database, FileNamePrefix + " 01 Data types.sql");
			Console.Write(" Ok\n");

			Console.Write("Scripting Functions.");
			ScriptUDFs(database, FileNamePrefix + " 02 Functions.sql");
			Console.Write(" Ok\n");

			Console.Write("Scripting Views.");
			ScriptViews(database, FileNamePrefix + " 03 Views.sql");
			Console.Write(" Ok\n");

			Console.Write("Scripting Tables.");
			ScriptTables(database, FileNamePrefix + " 04 Tables.sql");
			Console.Write(" Ok\n");

			Console.Write("Scripting Stored procedures.");
			ScriptSPs(database, FileNamePrefix + " 05 Stored procedures.sql");
			Console.Write(" Ok\n");


			if ( server.ConnectionContext.IsOpen )
			{
				server.ConnectionContext.Disconnect();
			}
		}
		static void StartWrite(string FileName)
		{
			Start = DateTime.UtcNow;
			StreamWriter sw = new StreamWriter(FileName, false, Encoding.Unicode);
			sw.Close();
		}
		static void WriteToFile(string FileName, StringCollection StringBlocks)
		{
			StreamWriter sw = new StreamWriter(FileName, true, Encoding.Unicode);
			foreach (string block in StringBlocks)
			{
				sw.WriteLine(block);
			}
			if (DateTime.UtcNow > Start.AddSeconds(1))
			{
				Console.Write(".");
				Start = DateTime.UtcNow;
			}
			sw.Close();
		}

		static void ScriptSPs(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);
			so.Add(ScriptOption.IncludeIfNotExists);
			
			StartWrite(fileName);
			foreach (StoredProcedure smo in database.StoredProcedures)
			{
				if (smo.IsSystemObject == false)
				{
					StringCollection sc = smo.Script(so);
					
					if(sc.Count == 3 || sc.Count == 4)
					{
						sc.Remove("SET ANSI_NULLS ON");
						sc.Remove("SET ANSI_NULLS OFF");
						sc.Remove("SET QUOTED_IDENTIFIER ON");
						sc.Remove("SET QUOTED_IDENTIFIER OFF");
						if(sc.Count == 2)
						{
							if(sc[1].Substring(0, 13) == "GRANT EXECUTE")
							{
								sc.RemoveAt(1);
							}
							else
							{
								throw new Exception("Stored Procedure has incorrect format.\"GRANT EXECUTE\" statement expected.");
							}
						}
						else if(sc.Count != 1)
						{
							throw new Exception("Stored Procedure has incorrect format.\"SET ANSI_NULLS\" or \"SET QUOTED_IDENTIFIER\" statement expected");
						}
					}
					else
					{
						throw new Exception("Stored Procedure has incorrect format. Expected count of blocks is 3 or 4.");
					}
					WriteToFile(fileName, sc);
				}
			}
		}

		static void ScriptTables(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);
			so.Add(ScriptOption.Triggers);
			so.Add(ScriptOption.Bindings);
			so.Add(ScriptOption.ClusteredIndexes);
			//so.Add(ScriptOption.WithDependencies);
			so.Add(ScriptOption.ExtendedProperties);

			StartWrite(fileName);
			foreach (Table smo in database.Tables)
			{
				if (smo.IsSystemObject == false)
				{
					StringCollection sc = smo.Script(so);
					WriteToFile(fileName, sc);
				}
			}
		}

		static void ScriptUDFs(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);
			
			StartWrite(fileName);
			foreach (UserDefinedFunction smo in database.UserDefinedFunctions)
			{
				if (smo.IsSystemObject == false)
				{
					StringCollection sc = smo.Script(so);
					WriteToFile(fileName, sc);
				}
			}
		}

		static void ScriptViews(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);

			StartWrite(fileName);
			foreach (View smo in database.Views)
			{
				if (smo.IsSystemObject == false)
				{
					StringCollection sc = smo.Script(so);
					WriteToFile(fileName, sc);
				}
			}
		}

		static void ScriptUDTs(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);

			StartWrite(fileName);
			foreach (UserDefinedDataType smo in database.UserDefinedDataTypes)
			{
				StringCollection sc = smo.Script(so);
				WriteToFile(fileName, sc);
			}
		}

		static void ScriptDefaults(Database database, string fileName)
		{
			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			so.Add(ScriptOption.Permissions);

			StartWrite(fileName);
			foreach (Default smo in database.Defaults)
			{
				StringCollection sc = smo.Script(so);
				WriteToFile(fileName, sc);
			}
		}

	}
}
