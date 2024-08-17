using System;
using System.Data;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace FoxProDbExtentionConnection
{
    public static class OleDbCommandExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public static void SkipValidateNullOnSave(this OleDbCommand command)
        {
#if Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using OleDbCommand newCommand = command.Connection.CreateCommand();
                newCommand.CommandType = CommandType.Text;
                newCommand.CommandText = "SET NULL OFF;";
                newCommand.ExecuteNonQuery();
            }
#endif
        }

        public static void SetDelete(this OleDbCommand command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var newCommand = command.Connection.CreateCommand();
                newCommand.CommandType = CommandType.Text;
                newCommand.CommandText = "SET DELETED ON;";
                newCommand.ExecuteNonQuery();
            }
        }
        static void Test()
        {
            /*
                EXECSCRIPT([set delete on] +CHR(13)+ [SET FILTER TO refpedido=179987]+CHR(13)+ [REPLACE estado WITH 108]+CHR(13)+[BROWSE LAST])
             */
            Console.WriteLine("Starting program execution...");

#if Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string connectionString = @"Provider=VFPOLEDB.1;Data Source=h:\dave\";
                using OleDbConnection connection = new(connectionString);
                using OleDbCommand scriptCommand = connection.CreateCommand();
                connection.Open();

                string vfpScript = @"[set delete on] +CHR(13)+ [SET FILTER TO refpedido=179987]+CHR(13)+ [REPLACE estado WITH 108]+CHR(13)+[BROWSE LAST]";

                scriptCommand.CommandType = CommandType.StoredProcedure;
                scriptCommand.CommandText = "ExecScript";
                scriptCommand.Parameters.Add("myScript", OleDbType.Char).Value = vfpScript;
                scriptCommand.ExecuteNonQuery();
            }
#endif
            Console.WriteLine("End program execution...");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}
