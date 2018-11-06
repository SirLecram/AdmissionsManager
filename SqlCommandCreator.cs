using AdmissionsManager.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;

namespace AdmissionsManager
{
    static class SqlCommandFilterCreator
    {
        //Dziala!
        /// <summary>
        /// Creates a command string which show actual table and sort it.
        /// </summary>
        /// <param name="actualPage">Actual view</param>
        /// <param name="orderBy">order by (optional)</param>
        /// <param name="sortCriterium"></param>
        /// <returns></returns>
        public static string CreateCommand(IDBConnectionStateGettable actualPage, string orderBy = null, 
            SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            string actualTableSqlDescription = actualPage.GetTableDescriptionToSql();

            string commandToReturn = "SELECT * FROM ";
            commandToReturn += actualTableSqlDescription;
            if (!string.IsNullOrEmpty(orderBy))
            {
                commandToReturn = CreateCommand(commandToReturn, orderBy, sortCriterium);
            }
            return commandToReturn;
        }
       /* public static string CreateCommand(string actualTableSqlDescription, string orderBy = null,
            SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            
            string commandToReturn = "SELECT * FROM ";
            commandToReturn += actualTableSqlDescription;
            if (!string.IsNullOrEmpty(orderBy))
            {
                commandToReturn = CreateCommand(commandToReturn, orderBy, sortCriterium);
            }
            return commandToReturn;
        }*/
        public static string CreateCommand(string actualCommandToOrder, string orderBy,
            SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            actualCommandToOrder = DeleteOrderFromCommand(actualCommandToOrder);
            actualCommandToOrder += " ORDER BY " + orderBy;
            if (sortCriterium == SortCriteria.Ascending)
                actualCommandToOrder += " ASC";
            else
                actualCommandToOrder += " DESC";

            return actualCommandToOrder;

        }

        public static string CreateCommand(IDBConnectionStateGettable actualPage, string searchIn,
            string searchedExpression, string sortBy = null, SortCriteria sortCriterium = SortCriteria.Ascending )
        {
            string commandToReturn = CreateCommand(actualPage);
            commandToReturn = AddWhereToCommandAndSort(commandToReturn, searchIn, searchedExpression, sortBy, sortCriterium);
            return commandToReturn;

        }

        public static string CreateCommand(string actualCommandToSearch, string searchIn, string searchedExpression,
            string sortBy = null, SortCriteria sortCriterium = SortCriteria.Ascending)
        {

            actualCommandToSearch = DeleteWhereFromCommand(actualCommandToSearch);
            actualCommandToSearch = DeleteOrderFromCommand(actualCommandToSearch);
            actualCommandToSearch = AddWhereToCommandAndSort(actualCommandToSearch, searchIn, searchedExpression, sortBy, sortCriterium);
            return actualCommandToSearch;
        }
        public static string CreateDeleteCommand(IDBConnectionStateGettable actualPage, string primaryKey, string nameOfPrimaryKey)
        {
            
            string commandToReturn = "DELETE FROM ";
            commandToReturn += actualPage.GetTableDescriptionToSql();
            commandToReturn += " WHERE " + nameOfPrimaryKey + " = '" + primaryKey + "'";
            return commandToReturn;
        }
        public static string CreateDeleteCommand(string sqlTableName, string primaryKey, string nameOfPrimaryKey)
        {
            string commandToReturn = "DELETE FROM ";
            commandToReturn += sqlTableName;
            commandToReturn += " WHERE " + nameOfPrimaryKey + " = '" + primaryKey + "'";
            return commandToReturn;
        }
        public static string CreateDeleteCommand(IDBConnectionStateGettable actualPage, int primaryKey, string nameOfPrimaryKey)
        {

            string commandToReturn = "DELETE FROM ";
            commandToReturn += actualPage.GetTableDescriptionToSql();
            commandToReturn += " WHERE " + nameOfPrimaryKey + " = " + primaryKey;
            return commandToReturn;
        }
        public static string CreateUpdateCommand(IDBConnectionStateGettable actualPage, string primaryKey, string nameOfPrimaryKey, 
            List<string> fieldsToUpdate, List<string> valueToSet)
        {
           
            string commandToReturn = "UPDATE " + actualPage.GetTableDescriptionToSql() + " SET " +
                fieldsToUpdate[0] + "='" + valueToSet[0] + "' WHERE " + nameOfPrimaryKey + " = '" + primaryKey + "'";
            if (commandToReturn.Contains(@"'NULL'"))
                commandToReturn = commandToReturn.Replace(@"'NULL'", "NULL");
            return commandToReturn;

        }
        public static string CreateNewRecordCommand(IDBConnectionStateGettable actualPage, List<string> valuesList, List<string> columnNames)
        {
            string commandToReturn = "INSERT INTO " + actualPage.GetTableDescriptionToSql() + " (";
            string lastColumn = columnNames.Last();
            string lastValue = valuesList.Last();
            foreach(string column in columnNames)
            {
                commandToReturn += column;
                if (column == lastColumn)
                    commandToReturn += ") VALUES (";
                else
                    commandToReturn += ", ";
            }
            foreach(object value in valuesList)
            {
                if (value.ToString() == "NULL")
                    commandToReturn += value;
                else
                    commandToReturn += "'" + value + "'";

                if (value.ToString() == lastValue)
                    commandToReturn += ")";
                else
                    commandToReturn += ", ";

            }
            return commandToReturn;
            
        }

        public static string ResetCommand(IDBConnectionStateGettable actualTable)
        {
            return CreateCommand(actualTable);
        }

        #region Help methods

        private static string AddWhereToCommandAndSort(string actualCommandToSearch, string searchIn, string searchedExpression,
            string sortBy = null, SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            actualCommandToSearch += " WHERE " + searchIn + " = '" + searchedExpression + "' ";
            if (!string.IsNullOrEmpty(sortBy))
            {
                actualCommandToSearch = CreateCommand(actualCommandToSearch, sortBy, sortCriterium);
            }
            return actualCommandToSearch;
        }
        private static string DeleteWhereFromCommand(string actualCommand)
        {
            if (actualCommand.Contains("WHERE"))
            {
                int indexOfWhere = actualCommand.IndexOf("WHERE");
                actualCommand = actualCommand.Remove(indexOfWhere - 1);
            }
            return actualCommand;
        }
        private static string DeleteOrderFromCommand(string actualCommand)
        {
            if (actualCommand.Contains("ORDER"))
            {
                int indexOfOrder = actualCommand.IndexOf("ORDER");
                actualCommand = actualCommand.Remove(indexOfOrder - 1);
            }
            return actualCommand;
        }
       
        #endregion

        #region Extensions

        public static string GetEnumDescription(this Enum e)
        {
            // DescriptionAttribute.GetCustomAttributes(e.GetType().GetMember(()))
            var enumMember = e.GetType().GetMember(e.ToString()).FirstOrDefault();
            DescriptionAttribute descriptionAttribute =
                enumMember == null
                ? default(DescriptionAttribute)
                : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute.Description;
        }
        public static T GetEnumFromDescription<T>(this string description)
        {
            // Found in: https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute

            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");

        }
        
        
        #endregion

    }
}
