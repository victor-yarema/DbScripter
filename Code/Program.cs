using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using MsSqlSmo;
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


				DateTime TimeBegin = DateTime.UtcNow;
				Console.WriteLine("Init - Begin.");

				List<object> Defaults_list = new List<object>();
				DefaultCollection Defaults = Db.Defaults;
				for (int i = 0; i < Defaults.Count; i++)
				{
					Defaults_list.Add(Defaults[i]);
				}
				List<object> Uddts_list = new List<object>();
				UserDefinedDataTypeCollection Uddts = Db.UserDefinedDataTypes;
				for (int i = 0; i < Uddts.Count; i++)
				{
					Uddts_list.Add(Uddts[i]);
				}
				List<object> Udtts_list = new List<object>();
				UserDefinedTableTypeCollection Udtts = Db.UserDefinedTableTypes;
				for (int i = 0; i < Udtts.Count; i++)
				{
					Udtts_list.Add(Udtts[i]);
				}
				//
				List<object> Tables_list = new List<object>();
				TableCollection Tables = Db.Tables;
				for (int i = 0; i < Tables.Count; i++)
				{
					if (!Tables[i].IsSystemObject)
					{
						Tables_list.Add(Tables[i]);
					}
				}
				List<object> Views_list = new List<object>();
				ViewCollection Views = Db.Views;
				for (int i = 0; i < Views.Count; i++)
				{
					if (!Views[i].IsSystemObject)
					{
						Views_list.Add(Views[i]);
					}
				}
				//
				List<object> Udfs_list = new List<object>();
				UserDefinedFunctionCollection Udfs = Db.UserDefinedFunctions;
				for (int i = 0; i < Udfs.Count; i++)
				{
					if (!Udfs[i].IsSystemObject)
					{
						Udfs_list.Add(Udfs[i]);
					}
				}
				List<object> Sps_list = new List<object>();
				StoredProcedureCollection Sps = Db.StoredProcedures;
				for (int i = 0; i < Sps.Count; i++)
				{
					if (!Sps[i].IsSystemObject)
					{
						Sps_list.Add(Sps[i]);
					}
				}

				string InitTimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
				Console.WriteLine("Init - End. TimeInterval = " + InitTimeInterval + " .");


				Script(Defaults_list.ToArray(), DbObjType.Default, FilenamePrefix + " 00 Defaults.sql");
				Script(Uddts_list.ToArray(), DbObjType.Uddt, FilenamePrefix + " 01 Uddts.sql");
				Script(Udtts_list.ToArray(), DbObjType.Udtt, FilenamePrefix + " 02 Udtts.sql");
				//
				Script(Tables_list.ToArray(), DbObjType.Table, FilenamePrefix + " 03 Tables.sql");
				Script(Views_list.ToArray(), DbObjType.View, FilenamePrefix + " 04 Views.sql");
				//
				Script(Udfs_list.ToArray(), DbObjType.Udf, FilenamePrefix + " 05 Udfs.sql");
				Script(Sps_list.ToArray(), DbObjType.Sp, FilenamePrefix + " 06 Sps.sql");

				Console.WriteLine();
				string TotalTimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
				Console.WriteLine("TotalTimeInterval = " + TotalTimeInterval + " .");
			}
			catch (Exception ex)
			{
				Console.Write("Exception:\r\n" + ex.ToString() + "\r\n");
			}
		}

		static void Script(object[] Objs, DbObjType Type, string Filename)
		{
			if (System.IO.File.Exists(Filename))
			{
				throw new Exception("File \"" + Filename + "\" already exists.");
			}

			DateTime TimeBegin = DateTime.UtcNow;
			Console.WriteLine(Type.ToString() + "s - Begin.");
			Console.WriteLine(Type.ToString() + "s - Objs.Length = " + Objs.Length + ".");

			ScriptingOptions so = new ScriptingOptions();
			so.Add(ScriptOption.DriAll);
			//so.Add(ScriptOption.Permissions);
			//so.Add(ScriptOption.IncludeIfNotExists);
			if (Type == DbObjType.Table)
			{
				so.Add(ScriptOption.Triggers);
				so.Add(ScriptOption.Bindings);
				so.Add(ScriptOption.ClusteredIndexes);
				//so.Add(ScriptOption.WithDependencies);
				so.Add(ScriptOption.ExtendedProperties);
			}

			using (StreamWriter File = new StreamWriter(Filename, false, Encoding.UTF8))
			{
				File.WriteLine();

				int ScriptMaxLen_Default = 0;
				int ScriptMaxLen_Uddt = 0;
				int ScriptMaxLen_Udtt = 0;
				//
				int ScriptMaxLen_Table = 0;
				int ScriptMaxLen_View = 0;
				//
				int ScriptMaxLen_Udf = 0;
				int ScriptMaxLen_Sp = 0;

				for (int i = 0; i < Objs.Length; i++)
				{
					StringCollection Script;
					switch (Type)
					{
						case DbObjType.Default:
							{
								Default DbObj = (Default)Objs[i];
								Script = DbObj.Script(so);
								if (ScriptMaxLen_Default < Script.Count)
								{
									ScriptMaxLen_Default = Script.Count;
								}
							} break;
						case DbObjType.Uddt:
							{
								UserDefinedDataType DbObj = (UserDefinedDataType)Objs[i];
								Script = DbObj.Script(so);
								if (ScriptMaxLen_Uddt < Script.Count)
								{
									ScriptMaxLen_Uddt = Script.Count;
								}
							} break;
						case DbObjType.Udtt:
							{
								UserDefinedTableType DbObj = (UserDefinedTableType)Objs[i];
								Script = DbObj.Script(so);
								if (ScriptMaxLen_Udtt < Script.Count)
								{
									ScriptMaxLen_Udtt = Script.Count;
								}
							} break;
						case DbObjType.Table:
							{
								Table DbObj = (Table)Objs[i];
								Script = DbObj.Script(so);

								if (Script[0] != SET_ANSI_NULLS_ON)
								{
									throw new Exception("Invalid format.");
								}
								Script.RemoveAt(0);
								if (Script[0] != SET_QUOTED_IDENTIFIER_ON)
								{
									throw new Exception("Invalid format.");
								}
								Script.RemoveAt(0);

								if (Script[0].Substring(0, CREATE_TABLE.Length) != CREATE_TABLE)
								{
									throw new Exception("Invalid format.");
								}

								for (int j = Script.Count - 1; j >= 2; j--)
								{
									if (Script[j].IndexOf(CHECK_CONSTRAINT) != -1)
									{
										Script.RemoveAt(j);
									}
								}
								for (int j = 0; j < Script.Count; j++)
								{
									if (Script[j].IndexOf(ALTER_TABLE) == 0 && Script[j].IndexOf(FOREIGN_KEY_) > 0)
									{
										Script[j] = Script[j].Replace(" " + FOREIGN_KEY, "\r\n" + FOREIGN_KEY + " ");
										Script[j] = Script[j].Replace("\r\n", "\r\n\t");
									}
								}

								if (ScriptMaxLen_Table < Script.Count)
								{
									ScriptMaxLen_Table = Script.Count;
								}

								if (Script.Count >= 16)
								{
									Console.WriteLine("Table \"" + DbObj.Name + "\" Script.Count = " + Script.Count + ".");
								}

								if (DbObj.DataSpaceUsed > 128 * 1000)
								{
									Console.WriteLine("Table \"" + DbObj.Name + "\" data size = " + DbObj.DataSpaceUsed / 1000 + " MB.");
								}
							} break;
						case DbObjType.View:
							{
								View DbObj = (View)Objs[i];
								Script = DbObj.Script(so);
								if (ScriptMaxLen_View < Script.Count)
								{
									ScriptMaxLen_View = Script.Count;
								}
							} break;
						case DbObjType.Udf:
							{
								UserDefinedFunction DbObj = (UserDefinedFunction)Objs[i];
								Script = DbObj.Script(so);
								if (ScriptMaxLen_Udf < Script.Count)
								{
									ScriptMaxLen_Udf = Script.Count;
								}
							} break;
						case DbObjType.Sp:
							{
								StoredProcedure DbObj = (StoredProcedure)Objs[i];
								Script = DbObj.Script(so);

								if (Script[0] != SET_ANSI_NULLS_ON)
								{
									throw new Exception("Invalid format.");
								}
								Script.RemoveAt(0);
								if (Script[0] != SET_QUOTED_IDENTIFIER_ON)
								{
									throw new Exception("Invalid format.");
								}
								Script.RemoveAt(0);

								if (ScriptMaxLen_Sp < Script.Count)
								{
									ScriptMaxLen_Sp = Script.Count;
								}
							} break;
						default:
							throw new Exception("Unknown Type = \"" + Type.ToString() + "\".");
					}

					StreamWriter_Write_StringCollection(File, Script);
					File.WriteLine();
					File.WriteLine();
					File.WriteLine();
					File.WriteLine();
				}

				switch (Type)
				{
					case DbObjType.Default:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Default + ".");
						} break;
					case DbObjType.Uddt:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Uddt + ".");
						} break;
					case DbObjType.Udtt:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Udtt + ".");
						} break;
					//
					case DbObjType.Table:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Table + ".");
						} break;
					case DbObjType.View:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_View + ".");
						} break;
					//
					case DbObjType.Udf:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Udf + ".");
						} break;
					case DbObjType.Sp:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Sp + ".");
						} break;
					default:
						throw new Exception("Unknown Type = \"" + Type.ToString() + "\".");
				}

			}
			string TimeInterval = TimeUtilities.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			Console.WriteLine(Type.ToString() + "s - End. TimeInterval = " + TimeInterval + " .");
		}

		const string SET_ANSI_NULLS_ON = "SET ANSI_NULLS ON";
		const string SET_QUOTED_IDENTIFIER_ON = "SET QUOTED_IDENTIFIER ON";
		const string CREATE_TABLE = "CREATE TABLE";
		const string CHECK_CONSTRAINT = "CHECK CONSTRAINT";
		const string ALTER_TABLE = "ALTER TABLE";
		const string FOREIGN_KEY = "FOREIGN KEY";
		const string FOREIGN_KEY_ = FOREIGN_KEY + "(";

		static void StreamWriter_Write_StringCollection(StreamWriter sw, StringCollection sc)
		{
			foreach (string s in sc)
			{
				sw.WriteLine(s);
			}
		}

	}
}
