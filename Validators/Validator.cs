using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdmissionsManager.Validators
{
    class Validator : IValidateIfInterfaceIsImplemented
    {
        public IProvideType TypeProvider { protected get; set; }

        public Validator() { }
        public Validator(IProvideType typeProvider) { TypeProvider = typeProvider; }

        public bool ValidateIfTypeImplementInterface(Type typeToCheck, string interfaceNameToCheck)
        {
            Type returnedType = typeToCheck.GetInterface(interfaceNameToCheck);
            if (returnedType == null)
                return false;
            return true;
        }

        public bool ValidateIfTypeImplementInterface(string typeName, string interfaceNameToCheck)
        {
            Type typeFromString = TypeProvider.GetTypeFromString(typeName);
            return ValidateIfTypeImplementInterface(typeFromString, interfaceNameToCheck);
        }

        public bool ValidateIfTypeImplementInterface(Type typeToCheck, Type interfaceToCheck)
        {
            string interfaceName = interfaceToCheck.Name;
            return ValidateIfTypeImplementInterface(typeToCheck, interfaceName);
        }
    }
}
