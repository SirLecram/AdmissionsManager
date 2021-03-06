﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Model
{
    internal abstract class SqlTable: ISqlTableModelable
    {
        // TODO: Dodac table do klas modelu
        public string PrimaryKey { get; protected set; }
        protected string PrimaryKeyName { get; set; }
        protected List<string> ColumnNames { get; set; }
        protected SqlTable()
        {
            PrimaryKey = "BRAK";
            PrimaryKeyName = "BRAK";
            ColumnNames = new List<string>();
        }
        public SqlTable(string primaryKey, string primaryKeyName, List<string> columnNames)
        {
            PrimaryKey = primaryKey;
            PrimaryKeyName = primaryKeyName;
            ColumnNames = columnNames;
        }

        public string GetPrimaryKey()
        {
            return PrimaryKey;
        }

        public string GetPrimaryKeyName()
        {
            return PrimaryKeyName;
        }

        public List<string> GetColumnNames()
        {
            return ColumnNames;
        }

        //STARA WERSJA:
        /*public abstract string PrimaryKeyNameToSql { get; protected set; }
        public abstract string GetPrimaryKey { get; }*/
    }
}
