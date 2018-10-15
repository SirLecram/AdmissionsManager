using AdmissionsManager.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

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
        public static string CreateCommand(IDatabaseConnectable actualPage, string orderBy = null, 
            SortCriteria sortCriterium = SortCriteria.Ascending)
        {
            Tabels actualTable = actualPage.GetModelType();

            string commandToReturn = "SELECT * FROM ";
            commandToReturn += actualTable.GetTableDescription();
            if(!string.IsNullOrEmpty(orderBy))
            {
                commandToReturn = CreateCommand(commandToReturn, orderBy, sortCriterium);
            }
            return commandToReturn;
        }

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

        public static string CreateCommand(IDatabaseConnectable actualPage, string searchIn,
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
        public static string CreateDeleteCommand(Tabels actualTable, string primaryKey, string nameOfPrimaryKey)
        {
            
            string commandToReturn = "DELETE FROM ";
            commandToReturn += actualTable.GetTableDescription();
            commandToReturn += " WHERE " + nameOfPrimaryKey + " = '" + primaryKey + "'";
            return commandToReturn;
        }
        public static string CreateDeleteCommand(Tabels actualTable, int primaryKey, string nameOfPrimaryKey)
        {

            string commandToReturn = "DELETE FROM ";
            commandToReturn += actualTable.GetTableDescription();
            commandToReturn += " WHERE " + nameOfPrimaryKey + " = " + primaryKey;
            return commandToReturn;
        }
        public static string CreateUpdateCommand(Tabels actualTable, string primaryKey, string nameOfPrimaryKey, 
            List<string> fieldsToUpdate, List<string> valueToSet)
        {
           
            string commandToReturn = "UPDATE " + actualTable.GetTableDescription() + " SET " +
                fieldsToUpdate[0] + "='" + valueToSet[0] + "' WHERE " + nameOfPrimaryKey + " = '" + primaryKey + "'";
            if (commandToReturn.Contains(@"'NULL'"))
                commandToReturn = commandToReturn.Replace(@"'NULL'", "NULL");
            return commandToReturn;

        }

        public static string ResetCommand(IDatabaseConnectable actualTable)
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
        /*private static string GetTableName(IDatabaseConnectable actualPage)
        {
            string commandToReturn;
            Tabels actualTable = actualPage.GetModelType();
            switch (actualTable)
            {
                case Tabels.Admissions:
                    commandToReturn = "Przyjecia ";
                    break;
                case Tabels.Patients:
                    commandToReturn = "Pacjenci ";
                    break;
                case Tabels.Doctors:
                    commandToReturn = "Lekarze ";
                    break;
                case Tabels.Diagnoses:
                    commandToReturn = "Diagnozy ";
                    break;
                case Tabels.Surgeries:
                    commandToReturn = "Operacje ";
                    break;
                case Tabels.Rooms:
                    commandToReturn = "Sale ";
                    break;
                default:
                    commandToReturn = null;
                    break;
            }
            
            return commandToReturn;

        }*/
        #endregion

        #region Extensions

        public static string GetTableDescription(this Enum e)
        {
            // DescriptionAttribute.GetCustomAttributes(e.GetType().GetMember(()))
            var enumMember = e.GetType().GetMember(e.ToString()).FirstOrDefault();
            DescriptionAttribute descriptionAttribute =
                enumMember == null
                ? default(DescriptionAttribute)
                : enumMember.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute.Description;
        }

        #endregion

    }
}
