using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace FoxProDbExtentionConnection
{
    internal class FoxDbContext : IFoxDbContext, IDisposable, IAsyncDisposable
    {
        #region Properties
        private readonly OleDbConnection _connection;
        private const int MIN_SUCCESSFUL_UPDATE = 1;
        private readonly string ConnectionString;
        #endregion

        #region Constructors
        private static async Task<OleDbConnection> FarmaDbContextAsync(string connectionString, CancellationToken cancellationToken = default)
        {
            var connection = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)? new OleDbConnection(connectionString):null;
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            return connection;
        }

        internal FoxDbContext(IOptions<FoxDbOptions> options)
        {
            ConnectionString = options.Value.DataFolderString;
            if (ConnectionString == string.Empty) // TODO: This is just for testing purpose. Should be deleted.
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }
            _connection = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? FarmaDbContextAsync(ConnectionString).Result:null;
        }

        internal FoxDbContext(string connectionString)
        {
            if (connectionString == string.Empty) // TODO: This is just for testing purpose. Should be deleted.
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            _connection = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? FarmaDbContextAsync(ConnectionString).Result : null;
        }
        #endregion

        #region FUNCTION
        public static DataSet Exec(string function, string database, Dictionary<string, int> parametros)
        {
#if Windows
            DataTable dataset = new();
            string fullconection = $"Provider = VFPOLEDB.1; Data Source = E:\\Share\\datatest\\DATA\\{database}.dbc;";
            using OleDbConnection connection = new(fullconection);
            using OleDbCommand command = connection.CreateCommand();
            command.CommandText = function;
            command.CommandType = CommandType.StoredProcedure;
            foreach (var items in parametros)
            {
                command.Parameters.Add(items.Key, items.Value);
            }
            OleDbDataAdapter adapter = new(command);
            adapter.Fill(dataset);
            return new DataSet();
#endif
        }
        #endregion

        #region Select Method
        public async Task<T> GetFirstAsync<T>(string query)
        {
            DataSet dataSet = new();
            using OleDbDataAdapter _adapter = new(query, _connection);
            _ = _adapter.Fill(dataSet);
            return await dataSet.FirstAsync<T>();
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string query)
        {
            DataSet dataSet = new();
            using OleDbDataAdapter _adapter = new(query, _connection);
            _ = _adapter.Fill(dataSet);
            return await dataSet.ToListAsync<T>();
        }

        public async Task<DataSet> GetDataSet(string query)
        {
            var dataSet = new DataSet();
            using (OleDbDataAdapter _adapter = new(query, _connection))
            {
                await Task.FromResult(_adapter.Fill(dataSet));
            }
            return dataSet;
        }

        private static IEnumerable<Dictionary<string, object>> Serialize(IDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            return results;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, IDataReader reader)
        {
            var result = new Dictionary<string, object>();
            foreach (var col in cols)
                result.Add(col, reader[col]);
            return result;
        }

        public async Task<IEnumerable<T>> GetReader<T>(string query)
        {
            try
            {
                DataSet ds = new();
                using (OleDbCommand command = new(query, _connection))
                {
                    _connection.Open();
                    using OleDbDataReader reader = command.ExecuteReader();
                    var r = Serialize(reader);
                    string json = JsonConvert.SerializeObject(r, Formatting.Indented);

                    while (await reader.ReadAsync())
                    {
                        ds.Tables.Add().Load(reader);
                    }
                }
                return await ds.ToListAsync<T>();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetList<T>(string query) where T : class
        {
            try
            {
                var dataSet = new DataSet();
                using (OleDbDataAdapter _adapter = new(query, _connection))
                {
                    await Task.FromResult(_adapter.Fill(dataSet));
                }

                return await dataSet.ToListAsync<T>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Update Method
        public async Task<int> Update(string query)
        {
            int result = 0;
            try
            {
                using (OleDbCommand command = new(query, _connection))
                {
                    result = await command.ExecuteNonQueryAsync();
                };

            }
            catch (OleDbException ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
        #endregion

        #region Insert Method
        public async Task Add(string query)
        {
            await Task.Run(async () =>
            {
                using OleDbCommand command = new(query, _connection);
                command.SkipValidateNullOnSave();
                var update = await command.ExecuteNonQueryAsync();
                if (update >= MIN_SUCCESSFUL_UPDATE)
                {
                    return;
                }
            });
        }
        #endregion

        #region Deleted Method
        public async Task Delete(string sql)
        {
            await Task.Run(async () =>
            {
                using OleDbCommand command = new(sql, _connection);
                if (await command.ExecuteNonQueryAsync() > 0)
                {
                    return;
                }
            });
        }
        #endregion

        #region Dinamic Insert
        //TODO: Dinamic Insert
        public OleDbDataAdapter DataInsert(string tabla, string[] columns)
        {
            string commandSql = $"INSERT INTO {tabla} (";
            string values = "VALUES(";
            using (var command = new OleDbCommand(commandSql + values, _connection))
            {
                for (int increment = 0; increment < columns.Length; increment++)
                {
                    commandSql += columns[increment] + ",";
                    values += "?,";
                    command.Parameters.Add(columns[increment], OleDbType.Char, 5, columns[increment]);
                }
                //command = new OleDbCommand($"INSERT INTO {tabla} (CustomerID, CompanyName) VALUES (?, ?)", _conn);
                commandSql = commandSql.TrimEnd(',');
                commandSql += ')';
                values = values.TrimEnd(',');
                values += ')';
                commandSql += $" {values}";

                string query = "INSERT INTO inv_diferenciaconteo VALUES(1,9055,2,0,4,5,CTOD('09/08/20'),1,'10:14 AM',0,553.87,'PAQUETE','037000862093',0,0)";
                command.CommandText = query;

                command.ExecuteNonQuery();
                _connection.Close();
            }
            return null;//_adapter;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
        protected virtual async ValueTask DisposeAsyncCore()
        {
            await Task.Run(() =>
            {
                _connection.Dispose();
            });
        }
        #endregion
    }
}