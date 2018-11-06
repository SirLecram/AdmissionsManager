using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.DataProviders
{
    class DatabaseInfoProvider : IDBInfoProvider
    {
        private string LastCommand { get; set; }
        private IEnumerable<string> LastColumnNames = new List<string>();
        private IDictionary<int, string> LastColumnTypes = new Dictionary<int, string>();

        public DatabaseInfoProvider()
        {
            LastCommand = string.Empty;
        }

        public async Task<IEnumerable<string>> GetColumnNamesFromTableAsync(string selectCommandText)
        {
            if (LastCommand != selectCommandText)
            {
                using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = selectCommandText;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            int columnAmount = reader.FieldCount;
                            List<string> columnNames = new List<string>();
                            for (int i = 0; i < columnAmount; i++)
                            {
                                columnNames.Add(reader.GetName(i));

                            }
                            LastColumnNames = columnNames;

                        }
                    }
                }
            }
            return LastColumnNames;
        }

        public async Task<IDictionary<int, string>> GetColumnTypeNamesAsync(string selectCommandText)
        {
            if (LastCommand != selectCommandText)
            {
                using (SqlConnection connection = new SqlConnection((App.Current as App).ConnectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = selectCommandText;
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            int columnAmount = reader.FieldCount;
                            Dictionary<int, string> typesDictionary = new Dictionary<int, string>();
                            for (int i = 0; i < columnAmount; i++)
                            {
                                typesDictionary.Add(i, reader.GetDataTypeName(i));
                            }
                            LastColumnTypes = typesDictionary;
                        }
                    }

                }
            }
            return LastColumnTypes;

        }

    }
}
