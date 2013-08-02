using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Management.Smo;
using MsSqlSmo;
using Time;
using SystemIo;


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
				string ThreadsNumStr = null;

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
						case "/t":
							{
								if (ThreadsNumStr != null)
								{
									throw new Exception("Multiple keys for ThreadsNumStr.");
								}
								if (!(arg_index + 1 < args.Length))
								{
									throw new Exception("Missing ThreadsNumStr after corresponding key.");
								}
								ThreadsNumStr = args[arg_index + 1];
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
				if (ThreadsNumStr == null)
				{
					ThreadsNumStr = "4";
				}
				int ThreadsNum = Int32.Parse(ThreadsNumStr);

				Console.WriteLine("Parameters:");
				Console.WriteLine("  ServerAddress = \"" + ServerAddress + "\".");
				Console.WriteLine("  DatabaseName = \"" + DatabaseName + "\".");
				Console.WriteLine("  FilenamePrefix = \"" + FilenamePrefix + "\".");
				Console.WriteLine("  ThreadsNum = \"" + ThreadsNum + "\".");


				DateTime TimeBegin = DateTime.UtcNow;
				Console.WriteLine("Init - Begin.");

				Database[] ThreadsDbs = new Database[ThreadsNum];
				for (int ThreadIndex = 0; ThreadIndex < ThreadsNum; ThreadIndex++)
				{
					Server ThreadSrv = new Server(ServerAddress);
					ThreadSrv.ConnectionContext.LoginSecure = true;
					//ThreadSrv.ConnectionContext.Login = "sa";
					//ThreadSrv.ConnectionContext.Password = "q";

					ThreadsDbs[ThreadIndex] = ThreadSrv.Databases[DatabaseName];
				}

				Server Srv = new Server(ServerAddress);
				Srv.ConnectionContext.LoginSecure = true;
				//Srv.ConnectionContext.Login = "sa";
				//Srv.ConnectionContext.Password = "q";

				Database Db = Srv.Databases[DatabaseName];


				List<DbObjsSameType> Arrs = new List<DbObjsSameType>();

				DefaultCollection Defaults = Db.Defaults;
				DbObjSimple[] DefaultsSimple = new DbObjSimple[Defaults.Count];
				for (int i = 0; i < Defaults.Count; i++)
				{
					DefaultsSimple[i] = new DbObjSimple(Defaults[i].Name);
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Default, DefaultsSimple));

				UserDefinedDataTypeCollection Uddts = Db.UserDefinedDataTypes;
				DbObjSimple[] UddtsSimple = new DbObjSimple[Uddts.Count];
				for (int i = 0; i < Uddts.Count; i++)
				{
					UddtsSimple[i] = new DbObjSimple(Uddts[i].Name);
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Uddt, UddtsSimple));

				UserDefinedTableTypeCollection Udtts = Db.UserDefinedTableTypes;
				DbObjSimple[] UdttsSimple = new DbObjSimple[Udtts.Count];
				for (int i = 0; i < Udtts.Count; i++)
				{
					UdttsSimple[i] = new DbObjSimple(Udtts[i].Name);
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Udtt, UdttsSimple));

				//
				TableCollection Tables = Db.Tables;
				DbObjSimple[] TablesSimple = new DbObjSimple[Tables.Count];
				for (int i = 0; i < Tables.Count; i++)
				{
					//if (!Tables[i].IsSystemObject)
					{
						TablesSimple[i] = new DbObjSimple(Tables[i].Name);
					}
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Table, TablesSimple));

				ViewCollection Views = Db.Views;
				DbObjSimple[] ViewsSimple = new DbObjSimple[Views.Count];
				for (int i = 0; i < Views.Count; i++)
				{
					//if (!Views[i].IsSystemObject)
					{
						ViewsSimple[i] = new DbObjSimple(Views[i].Name);
					}
				}
				Arrs.Add(new DbObjsSameType(DbObjType.View, ViewsSimple));

				//
				UserDefinedFunctionCollection Udfs = Db.UserDefinedFunctions;
				DbObjSimple[] UdfsSimple = new DbObjSimple[Udfs.Count];
				for (int i = 0; i < Udfs.Count; i++)
				{
					//if (!Udfs[i].IsSystemObject)
					{
						UdfsSimple[i] = new DbObjSimple(Udfs[i].Name);
					}
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Udf, UdfsSimple));

				StoredProcedureCollection Sps = Db.StoredProcedures;
				DbObjSimple[] SpsSimple = new DbObjSimple[Sps.Count];
				for (int i = 0; i < Sps.Count; i++)
				{
					//if (!Sps[i].IsSystemObject)
					{
						SpsSimple[i] = new DbObjSimple(Sps[i].Name);
					}
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Sp, SpsSimple));

				//
				FullTextCatalogCollection Ftcs = Db.FullTextCatalogs;
				DbObjSimple[] FtcsSimple = new DbObjSimple[Ftcs.Count];
				for (int i = 0; i < Ftcs.Count; i++)
				{
					FtcsSimple[i] = new DbObjSimple(Ftcs[i].Name);
				}
				Arrs.Add(new DbObjsSameType(DbObjType.Ftc, FtcsSimple));

				string InitTimeInterval = TimeUtils.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
				Console.WriteLine("Init - End. TimeInterval = " + InitTimeInterval + " .");


				//DateTime Threads00_TimeBegin = DateTime.UtcNow;
				//Console.WriteLine("Threads00 - Begin.");

				for (int ArrIndex = 0; ArrIndex < Arrs.Count; ArrIndex++)
				{
					DbObjsSameType CurArr = Arrs[ArrIndex];

					DateTime CurArr_TimeBegin = DateTime.UtcNow;
					Console.WriteLine(CurArr.Type.ToString() + "s - Begin.");

					int BlockLen = CurArr.Items.Length / ThreadsNum;
					int ReamainingItemsLen = CurArr.Items.Length % ThreadsNum;

					ParameterizedThreadStart Thread_DbObjsSetIsSystem_Pts = new ParameterizedThreadStart(Thread_DbObjsSetIsSystem.ThreadMain);
					AutoResetEvent[] Threads_Events = new AutoResetEvent[ThreadsNum];
					Thread_DbObjsSetIsSystem_Param[] Params = new Thread_DbObjsSetIsSystem_Param[ThreadsNum];
					for (int ThreadIndex = 0; ThreadIndex < ThreadsNum; ThreadIndex++)
					{
						int ExtraItemsLen = 0;
						if ((ThreadIndex + 1) == ThreadsNum)
						{
							ExtraItemsLen = ReamainingItemsLen;
						}
						Threads_Events[ThreadIndex] = new AutoResetEvent(false);
						Params[ThreadIndex] = new Thread_DbObjsSetIsSystem_Param(
							Threads_Events[ThreadIndex],
							null,
							ThreadsDbs[ThreadIndex],
							CurArr,
							ThreadIndex * BlockLen,
							(ThreadIndex + 1) * BlockLen + ExtraItemsLen
							);
						Thread CurThread = new Thread(Thread_DbObjsSetIsSystem_Pts);
						CurThread.Start(Params[ThreadIndex]);
					}
					WaitHandle.WaitAll(Threads_Events);
					bool ThreadException = false;
					for (int ThreadIndex = 0; ThreadIndex < ThreadsNum; ThreadIndex++)
					{
						if (Params[ThreadIndex].Result != null)
						{
							ThreadException = true;
							Console.WriteLine("CurArr.Type = " + CurArr.Type + ", ThreadIndex = " + ThreadIndex + ", Exception:");
							Console.WriteLine(Params[ThreadIndex].Result.ToString());
						}
					}
					if (ThreadException)
					{
						throw new Exception("Thread(s) Exception(s).");
					}

					List<DbObjSimple> CurArrItemsNonSystem = new List<DbObjSimple>(CurArr.Items.Length);
					for (int i = 0; i < CurArr.Items.Length; i++)
					{
						if (!CurArr.Items[i].IsSystem)
						{
							CurArrItemsNonSystem.Add(CurArr.Items[i]);
						}
					}
					CurArr.Items = CurArrItemsNonSystem.ToArray();

					string CurArr_TimeInterval = TimeUtils.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - CurArr_TimeBegin);
					Console.WriteLine(CurArr.Type.ToString() + "s - End. TimeInterval = " + CurArr_TimeInterval + " .");
				}

				//string Threads00_TimeInterval = TimeUtils.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - Threads00_TimeBegin);
				//Console.WriteLine("Threads00 - End. TimeInterval = " + Threads00_TimeInterval + " .");



				string[] DbObjTypesFilenames = new string[] {
					"00 Defaults.sql",
					"01 Uddts.sql",
					"02 Udtts.sql",
					//
					"03 Tables.sql",
					"04 Views.sql",
					//
					"05 Udfs.sql",
					"06 Sps.sql",
					//
					"07 Ftc.sql",
				};

				for (int ArrIndex = 0; ArrIndex < Arrs.Count; ArrIndex++)
				{
					DbObjsSameType CurArr = Arrs[ArrIndex];
					Script(
						CurArr.Items,
						CurArr.Type,
						FilenamePrefix + " " + DbObjTypesFilenames[ArrIndex]
						);
				}


				Console.WriteLine();
				string TotalTimeInterval = TimeUtils.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
				Console.WriteLine("TotalTimeInterval = " + TotalTimeInterval + " .");
			}
			catch (Exception ex)
			{
				Console.Write("Exception:\r\n" + ex.ToString() + "\r\n");
			}
		}

		static void Script(DbObjSimple[] Objs, DbObjType Type, string Filename)
		{
			if (System.IO.File.Exists(Filename))
			{
				throw new Exception("File \"" + Filename + "\" already exists.");
			}

			//DateTime TimeBegin = DateTime.UtcNow;
			//Console.WriteLine(Type.ToString() + "s - Begin.");
			Console.WriteLine(Type.ToString() + "s - Objs.Length = " + Objs.Length + ".");

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
				//
				int ScriptMaxLen_Ftc = 0;

				for (int i = 0; i < Objs.Length; i++)
				{
					DbObjSimple DbObj = Objs[i];
					if (DbObj.IsSystem)
					{
						continue;
					}
					StringCollection Script = DbObj.Script;
					switch (Type)
					{
						case DbObjType.Default:
							{
								if (ScriptMaxLen_Default < Script.Count)
								{
									ScriptMaxLen_Default = Script.Count;
								}
							} break;
						case DbObjType.Uddt:
							{
								if (ScriptMaxLen_Uddt < Script.Count)
								{
									ScriptMaxLen_Uddt = Script.Count;
								}
							} break;
						case DbObjType.Udtt:
							{
								if (ScriptMaxLen_Udtt < Script.Count)
								{
									ScriptMaxLen_Udtt = Script.Count;
								}
							} break;
						//
						case DbObjType.Table:
							{
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
									Console.WriteLine("  Table \"" + DbObj.Name + "\" Script.Count = " + Script.Count + ".");
								}

								/*if (DbObj.DataSpaceUsed > 128 * 1000)
								{
									Console.WriteLine("Table \"" + DbObj.Name + "\" data size = " + DbObj.DataSpaceUsed / 1000 + " MB.");
								}*/
							} break;
						case DbObjType.View:
							{
								if (ScriptMaxLen_View < Script.Count)
								{
									ScriptMaxLen_View = Script.Count;
								}
							} break;
						//
						case DbObjType.Udf:
							{
								if (ScriptMaxLen_Udf < Script.Count)
								{
									ScriptMaxLen_Udf = Script.Count;
								}
							} break;
						case DbObjType.Sp:
							{
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
						//
						case DbObjType.Ftc:
							{
								if (ScriptMaxLen_Ftc < Script.Count)
								{
									ScriptMaxLen_Ftc = Script.Count;
								}
							} break;
						default:
							throw new Exception("Unknown Type = \"" + Type.ToString() + "\".");
					}

					File.WriteLine("---------------- " + DbObj.Name);
					StreamWriterUtils.Write_StringCollection(File, Script);
					File.WriteLine();
					File.WriteLine();
					File.WriteLine();
					File.WriteLine();
				}

				/*
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
					//
					case DbObjType.Ftc:
						{
							Console.WriteLine(Type.ToString() + "s - ScriptMaxLen = " + ScriptMaxLen_Ftc + ".");
						} break;
					default:
						throw new Exception("Unknown Type = \"" + Type.ToString() + "\".");
				}
				*/

			}
			//string TimeInterval = TimeUtils.IntervalToStringHHHMMSSLLLDec(DateTime.UtcNow - TimeBegin);
			//Console.WriteLine(Type.ToString() + "s - End. TimeInterval = " + TimeInterval + " .");
		}

		const string SET_ANSI_NULLS_ON = "SET ANSI_NULLS ON";
		const string SET_QUOTED_IDENTIFIER_ON = "SET QUOTED_IDENTIFIER ON";
		const string CREATE_TABLE = "CREATE TABLE";
		const string CHECK_CONSTRAINT = "CHECK CONSTRAINT";
		const string ALTER_TABLE = "ALTER TABLE";
		const string FOREIGN_KEY = "FOREIGN KEY";
		const string FOREIGN_KEY_ = FOREIGN_KEY + "(";

	}
}
