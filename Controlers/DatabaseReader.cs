﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Controlers
{
    class DatabaseReader
    {
        private List<ISqlTableModelable> _ModelsList { get; set; }
        private IValidateIfInterfaceIsImplemented Validator { get; set; }
        public IReadOnlyList<ISqlTableModelable> LastReadedModels { get => _ModelsList;  }
        public DatabaseReader(IValidateIfInterfaceIsImplemented validator)
        {
            _ModelsList = new List<ISqlTableModelable>();
            Validator = validator;
            
        }
        /// <summary>
        /// Reads the data from the database and write models in LastReadedModels. The model have to implements ISqlTableModelable.
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="sqlCommand">Command to execute</param>
        /// <param name="typeOfModel">Type of model</param>
        /// <returns>True if reading ends in right way</returns>
        public async Task<bool> ReadDataFromDatabase(string connectionString, string sqlCommand, Type typeOfModel)
        {
            if(!Validator.ValidateIfTypeImplementInterface(typeOfModel, "ISqlTableModelable"))
                throw new ArgumentException("Wrong model type! Model have to implement ISqlTableModelable interface.");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                }
                catch (Exception e)
                {
                    throw new Exception("Nie ma możliwości połączyć się z DB.");
                    
                }
                if (connection.State == ConnectionState.Open)
                {

                    using (SqlCommand cmd = connection.CreateCommand())
                    {

                        cmd.CommandText = sqlCommand;
                        // TODO: Dodac validator sprawdzajacy komende?
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {

                            int fieldCount = reader.FieldCount;
                            _ModelsList.Clear();
                            //List<ISqlTableModelable> sqlTablesModels = new List<ISqlTableModelable>();
                            while (await reader.ReadAsync())
                            {
                                List<string> valueList = new List<string>();
                                for (int i = 0; i < fieldCount; i++)
                                {
                                    if (String.IsNullOrEmpty(reader[i].ToString()))
                                        valueList.Add("NULL");
                                    else
                                        valueList.Add(reader[i].ToString());
                                }
                                var model = Activator.CreateInstance(typeOfModel, valueList);

                                _ModelsList.Add(model as ISqlTableModelable);
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            
            
        }
    }
}