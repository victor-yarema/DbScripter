using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Time;


namespace DbScripter
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string ServerAddress = null;
				string DatabaseName = null;
				string FilenamePrefix = null;

				for (int arg_index = 0; arg_index < args.Length; arg_index++)
				{
					switch (args[arg_index].ToLower())
					{
						case "/s":
							{
								if (ServerAddress != null)
								{
									throw new Exception("Multiple keys for ServerAddress.");
								}
								if (!(arg_index + 1 < args.Length))
								{
									throw new Exception("Missing ServerAddress after corresponding key.");
								}
								ServerAddress = args[arg_index + 1];
							} break;
						case "/d":
							{
								if (DatabaseName != null)
								{
									throw new Exception("Multiple keys for DatabaseName.");
								}
								if (!(arg_index + 1 < args.Length))
								{
									throw new Exception("Missing DatabaseName after corresponding key.");
								}
								DatabaseName = args[arg_index + 1];
							} break;
						case "/f":
							{
								if (FilenamePrefix != null)
								{
									throw new Exception("Multiple keys for FilenamePrefix.");
								}
								if (!(arg_index + 1 < args.Length))
								{
									throw new Exception("Missing FilenamePrefix after corresponding key.");
								}
								FilenamePrefix = args[arg_index + 1];
							} break;
					}
				}

				if (ServerAddress == null)
				{
					throw new Exception("ServerAddress undefined.");
				}
				if (DatabaseName == null)
				{
					throw new Exception("DatabaseName undefined.");
				}
				if (FilenamePrefix == null)
				{
					throw new Exception("FilenamePrefix undefined.");
				}

				Server Server_ = new Server(ServerAddress);
				Server_.ConnectionContext.LoginSecure = true;
				//Server_.ConnectionContext.Login = "sa";
				//Server_.ConnectionContext.Password = "q";

				Database Db = Server_.Databases[DatabaseName];

				ScriptDefaults(Db, FilenamePrefix + " 00 Defaults.sql");
				ScriptUddts(Db, FilenamePrefix + " 01 Uddts.sql");
				ScriptUdtts(Db, FilenamePrefix + " 02 Udtts.sql");
				ScriptTables(Db, FilenamePrefix + " 03 Tables.sql");
				ScriptViews(Db, FilenamePrefix + " 04 Views.sql");
				ScriptUdfs(Db, FilenamePrefix + " 05 Udfs.sql");
				ScriptSps(Db, FilenamePrefix + " 06 Sps.sql");
			}
			catch (Exception ex)
			{
				Console.Write("Exception:\r\n" + ex.ToString() + "\r\n");
			}
		}

		static void ScriptDefaults(Database Db, string Filename)
		{
			DefaultCollection Defaults = Db.Defaults;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Defaults - Begin.");
			Console.WriteLine("Scripting Defaults - Defaults.Count = " + Defaults.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Defaults.Count; i++)
				{
					Default DbObj = Defaults[i];
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Defaults - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptUddts(Database Db, string Filename)
		{
			UserDefinedDataTypeCollection Uddts = Db.UserDefinedDataTypes;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Uddts - Begin.");
			Console.WriteLine("Scripting Uddts - Uddts.Count = " + Uddts.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Uddts.Count; i++)
				{
					UserDefinedDataType DbObj = Uddts[i];
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Uddts - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptUdtts(Database Db, string Filename)
		{
			UserDefinedTableTypeCollection Udtts = Db.UserDefinedTableTypes;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Udtts - Begin.");
			Console.WriteLine("Scripting Udtts - Udtts.Count = " + Udtts.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Udtts.Count; i++)
				{
					UserDefinedTableType DbObj = Udtts[i];
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Udtts - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptTables(Database Db, string Filename)
		{
			TableCollection Tables = Db.Tables;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Tables - Begin.");
			Console.WriteLine("Scripting Tables - Tables.Count = " + Tables.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);
			so.Add(ScriptOption.Triggers);
			so.Add(ScriptOption.Bindings);
			so.Add(ScriptOption.ClusteredIndexes);
			//so.Add(ScriptOption.WithDependencies);
			so.Add(ScriptOption.ExtendedProperties);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Tables.Count; i++)
				{
					Table DbObj = Tables[i];
					if (DbObj.IsSystemObject)
					{
						continue;
					}
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Tables - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptViews(Database Db, string Filename)
		{
			ViewCollection Views = Db.Views;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Views - Begin.");
			Console.WriteLine("Scripting Views - Views.Count = " + Views.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Views.Count; i++)
				{
					View DbObj = Views[i];
					if (DbObj.IsSystemObject)
					{
						continue;
					}
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Views - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptUdfs(Database Db, string Filename)
		{
			UserDefinedFunctionCollection Udfs = Db.UserDefinedFunctions;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Udfs - Begin.");
			Console.WriteLine("Scripting Udfs - Udfs.Count = " + Udfs.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Udfs.Count; i++)
				{
					UserDefinedFunction DbObj = Udfs[i];
					if (DbObj.IsSystemObject)
					{
						continue;
					}
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);
					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Udfs - End. TimeInterval = " + TimeInterval + " .");
		}

		static void ScriptSps(Database Db, string Filename)
		{
			StoredProcedureCollection Sps = Db.StoredProcedures;
			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine("Scripting Sps - Begin.");
			Console.WriteLine("Scripting Sps - Sps.Count = " + Sps.Count + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);
			//so.Add(ScriptOption.IncludeIfNotExists);

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();
				for (int i = 0; i < Sps.Count; i++)
				{
					StoredProcedure DbObj = Sps[i];
					if (DbObj.IsSystemObject)
					{
						continue;
					}
					File.WriteLine();
					File.WriteLine();
					StringCollection Script = DbObj.Script(so);

					/*
					if (!(Script.Count == 3 || Script.Count == 4))
					{
						throw new Exception("Stored Procedure has incorrect format. Expected count of blocks is 3 or 4.");
					}
					Script.Remove("SET ANSI_NULLS ON");
					Script.Remove("SET ANSI_NULLS OFF");
					Script.Remove("SET QUOTED_IDENTIFIER ON");
					Script.Remove("SET QUOTED_IDENTIFIER OFF");
					if (Script.Count == 2)
					{
						if (Script[1].Substring(0, 13) == "GRANT EXECUTE")
						{
							Script.RemoveAt(1);
						}
						else
						{
							throw new Exception("Stored Procedure has incorrect format. \"GRANT EXECUTE\" statement expected.");
						}
					}
					else if (Script.Count != 1)
					{
						throw new Exception("Stored Procedure has incorrect format. \"SET ANSI_NULLS\" or \"SET QUOTED_IDENTIFIER\" statement expected.");
					}
					*/

					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
				}
			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine("Scripting Sps - End. TimeInterval = " + TimeInterval + " .");
		}

		static void StreamWriter_Write_StringCollection(StreamWriter sw, StringCollection sc)
		{
			foreach (string s in sc)
			{
				sw.WriteLine(s);
			}
			sw.WriteLine();
		}

	}
}
