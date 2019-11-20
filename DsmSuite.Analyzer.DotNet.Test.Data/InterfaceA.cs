namespace DsmSuite.Analyzer.DotNet.Test.Data
{
    public interface InterfaceA
    {
        ReturnType MethodA(ParameterType a, GenericParameterType<GenericParameterTypeParameter> b, ParameterEnum c);
        GenericReturnType<GenericReturnTypeParameter> MethodB();
        ReturnEnum MethodC();
    }
}
