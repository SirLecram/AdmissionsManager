using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdmissionsManager
{
    public interface IDatabaseConnectable
    {
        ObservableCollection<ISqlTableModelable> RecordsList { get; }
        bool IsConnectedToDb { get; }
        bool IsDataLoaded { get; }
        Task<bool> ConnectToDatabaseAsync();
        Tabels GetModelType();
        void UnloadPage();
    }
   /* public interface IDatabaseModelable
    {
        string PrimaryKeyNameToSql { get; set; }
        string GetPrimaryKey { get; }
    }*/
    public interface ISqlTableModelable : IPrimaryKeyGetable
    {
        List<string> GetColumnNames();
    }
    public interface IPrimaryKeyGetable
    {
        string GetPrimaryKey();
        string GetPrimaryKeyName();
    }

    public interface INavigator
    {
        bool IsParameterSet { get; }
        void SetParameter(object parameter);
        void RemoveParameters();
        IPageNavigateable ChangeFrame(Type typeOfPage, Frame navigationFrame);
    }

    public interface IPageNavigateable
    {
        void UnloadPage();
    }
    public interface IValidateIfInterfaceIsImplemented
    {
        /// <summary>
        /// Validates if type implement interface. TypeProvider have to be initialized before use this overloaded method.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="interfaceNameToCheck"></param>
        /// <returns></returns>
        bool ValidateIfTypeImplementInterface(string typeName, string interfaceNameToCheck);
        bool ValidateIfTypeImplementInterface(Type typeToCheck, string interfaceNameToCheck);
        bool ValidateIfTypeImplementInterface(Type typeToCheck, Type interfaceToCheck);
    }
    
    /*public interface IValidateData : IValidateIfInterfaceIsImplemented
    {

    }*/
    public interface IProvideType
    {
        void RegisterType(Type typeToRegister);
        Type GetTypeFromString(string typeName);
    }
}
