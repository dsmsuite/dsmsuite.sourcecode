using System;
using System.Collections.Generic;

namespace DsmSuite.Analyzer.DotNet.Test.Data
{
    public class MainType : BaseType, INterfaceA
    {
        delegate int MyDelegate(List<DelegateGenericParameter> parameter);

        public event EventHandler<EventsArgsGenericParameter> GenericEvent;

        public MainType()
        {
            List<DelegateGenericParameter> argument = new List<DelegateGenericParameter>();
            MyDelegate myDelegate = x => x.Count + x.Count;

            myDelegate(argument);
        }

        private FieldType _fieldType;
        private NestedType _nestedFieldType;

        private GenericFieldType<GenericFieldTypeParameter> _genericFieldType;

        public PropertyType PropertyA { get; set; }
        public GenericPropertyType<GenericPropertyTypeParameter> PropertyB { get; set; }
        public PropertyEnum PropertyC { get; set; }

        public class NestedType
        {

        };

        public ReturnType MethodA(ParameterType a, GenericParameterType<GenericParameterTypeParameter> b, ParameterEnum c)
        {
            MethodVariableType d = new MethodVariableType();
            return null;
        }

        public GenericReturnType<GenericReturnTypeParameter> MethodB()
        {
            return null;
        }

        public ReturnEnum MethodC()
        {
            return ReturnEnum.A;
        }
    }
}
