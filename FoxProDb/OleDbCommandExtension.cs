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
            using OleDbCommand newCommand = command.Connection.CreateCommand();
                newCommand.CommandType = CommandType.Text;
                newCommand.CommandText = "SET NULL OFF;";
                newCommand.ExecuteNonQuery();
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
    }
}
