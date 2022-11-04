using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxProDbExtentionConnection
{
    public interface IFoxDbContext
    {
        Task Add(string query);
        Task Delete(string sql);
        Task<DataSet> GetDataSet(string query);
        Task<T> GetFirstAsync<T>(string query);
        string GetFirstAsyncJson(string query);
        Task<IEnumerable<T>> GetList<T>(string query) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(string query);
        string GetListAsyncJson(string query);
        Task<IEnumerable<T>> GetReader<T>(string query);
        Task<int> Update(string query);
    }
}
