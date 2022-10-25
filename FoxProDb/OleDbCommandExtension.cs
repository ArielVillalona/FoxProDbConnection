using System;
using System.Data;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace FoxProDbExtentionConnection
{
    public static class OleDbCommandExtension
    {
        /// <summary>
        /// ssssssssssssssssssssssssssssssssssssssssss
        /// </summary>
        /// <param name="command"></param>
        public static void SkipValidateNullOnSave(this OleDbCommand command)
        {
            /**
             * Intelifarma database ins't well configurated. Any value can't be null. We can allow null setting this command below.
             * http://vfphelp.com/vfp9/_59k0sp3t9.htm
             */
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
        static void test()
        {
            /*
                EXECSCRIPT([set delete on] +CHR(13)+ [SET FILTER TO refpedido=179987]+CHR(13)+ [REPLACE estado WITH 108]+CHR(13)+[BROWSE LAST])
             */
            Console.WriteLine("Starting program execution...");

            string connectionString = @"Provider=VFPOLEDB.1;Data Source=h:\dave\";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand scriptCommand = connection.CreateCommand())
                {
                    connection.Open();

                    string vfpScript = @"SET EXCLUSIVE ON
                                DELETE FROM test WHERE id = 5
                                PACK";

                    scriptCommand.CommandType = CommandType.StoredProcedure;
                    scriptCommand.CommandText = "ExecScript";
                    scriptCommand.Parameters.Add("myScript", OleDbType.Char).Value = vfpScript;
                    scriptCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine("End program execution...");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}
