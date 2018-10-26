using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AdmissionsManager.Controlers
{
    class ViewNavigator : INavigator
    {
        public IPageNavigateable ActualPage { get; protected set; }
        public bool IsParameterSet { get => Parameters == null ? false : true; }
        private object Parameters { get; set; }
        private IValidateIfInterfaceIsImplemented Validator = null;
        // TODO: Przemyslec czy potrzebne actualPage
        protected ViewNavigator()
        { Parameters = null; ActualPage = null; }
        public ViewNavigator(IValidateIfInterfaceIsImplemented validator, IPageNavigateable actualPage, object parametersToPage = null)
        {
            ActualPage = actualPage;
            Validator = validator;
            Parameters = parametersToPage;
        }
        /*public ViewNavigator(IPageNavigateable actualPage, IValidateIfInterfaceIsImplemented validator, object parametersToPage)
            : this( validator, actualPage)
        {
            Parameters = parametersToPage;
        }*/

        public void SetParameter(object parameter)
        {
            Parameters = parameter;
        }
        public void RemoveParameters()
        {
            Parameters = null;
        }
        
        public IPageNavigateable ChangeFrame(Type typeOfPage, Frame navigationFrame)
        {
            if (!Validator.ValidateIfTypeImplementInterface(typeOfPage, "IPageNavigateable"))
                throw new ArgumentException("Wrong page type! Page have to implement IPageNavigateable.");

            ActualPage.UnloadPage();
            if(IsParameterSet)
                navigationFrame.Navigate(typeOfPage, Parameters);
            else
                navigationFrame.Navigate(typeOfPage);
            return ActualPage = navigationFrame.Content as IPageNavigateable;
        }
    }
}
