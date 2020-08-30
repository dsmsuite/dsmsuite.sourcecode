using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DsmSuite.Analyzer.DotNet.Lib;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using Mono.Cecil;

namespace DsmSuite.Analyzer.DotNet.Analysis
{
    /// <summary>
    /// .Net code analyzer which uses Mono.Cecil to analyze dependencies between types in .Net binaries
    /// </summary>
    public class AssemblyFile
    {
        private readonly ICollection<string> _ignoredNames;
        private readonly IProgress<ProgressInfo> _progress;
        private readonly Dictionary<string, FileInfo> _typeAssemblyInfoList = new Dictionary<string, FileInfo>();
        private readonly IList<TypeDefinition> _typeList = new List<TypeDefinition>();

        public AssemblyFile(string filename, ICollection<string> ignoredNames, IProgress<ProgressInfo> progress)
        {
            FileInfo = new FileInfo(filename);
            _ignoredNames = ignoredNames;
            _progress = progress;
        }

        public List<AssemblyType> Types { get; } = new List<AssemblyType>();

        public List<AssemblyTypeRelation> Relations { get; } = new List<AssemblyTypeRelation>();


        public FileInfo FileInfo { get; }

        public bool Exists => FileInfo.Exists;

        public bool IsAssembly
        {
            get
            {
                try
                {
                    AssemblyName.GetAssemblyName(FileInfo.FullName); // Gives exception when no assembly

                    return ((FileInfo.Extension == ".exe") || (FileInfo.Extension == ".dll")) &&
                            !FileInfo.Name.EndsWith(".vshost.exe");
                }
                catch
                {
                    return false;
                }
            }
        }


        public void FindTypes(ReaderParameters readerParameters)
        {
            try
            {
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(FileInfo.FullName, readerParameters);

                AssemblyName.GetAssemblyName(FileInfo.FullName);

                foreach (ModuleDefinition module in assembly.Modules)
                {
                    var moduleTypes = module.Types;

                    foreach (TypeDefinition typeDecl in moduleTypes)
                    {
                        if ((typeDecl != null) && (typeDecl.Name != "<Module>") && !isClrSupportType(typeDecl))
                        {
                            AnalyseTypeElements(FileInfo, typeDecl);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Analysis failed assembly={FileInfo.FullName} failed", e);
            }
        }

        private bool isClrSupportType(TypeDefinition typeDecl)
        {
            return typeDecl.Name.StartsWith("_") ||
                   typeDecl.Name.StartsWith("tag") ||
                   typeDecl.Name.StartsWith("IMPORT_OBJECT_") ||
                   typeDecl.Name.StartsWith("SYSGEO") ||
                   typeDecl.Name == "IMAGE_AUX_SYMBOL_TYPE" ||
                   typeDecl.Name == "ReplacesCorHdrNumericDefines" ||
                   typeDecl.Name == "ORIENTATION_PREFERENCE" ||
                   typeDecl.Name == "SYSNLS_FUNCTION" ||
                   typeDecl.Name == "RPC_ADDRESS_CHANGE_TYPE" ||
                   typeDecl.Name == "RpcProxyPerfCounters" ||
                   typeDecl.Name == "CO_MARSHALING_CONTEXT_ATTRIBUTES" ||
                   typeDecl.Name == "CWMO_FLAGS" ||
                   typeDecl.Name == "VARENUM" ||
                   typeDecl.Name == "PIDMSI_STATUS_VALUE" ||
                   typeDecl.Name == "ValidatorFlags" ||
                   typeDecl.Name == "ETaskType" ||
                   typeDecl.Name == "ISA_AVAILABILITY" ||
                   typeDecl.Name == "IUnknown" ||
                   typeDecl.Name == "HWND__" ||
                   typeDecl.Name == "ICLRRuntimeHost" ||
                   typeDecl.Name == "HINSTANCE__" ||
                   typeDecl.Name == "HDC__" ||
                   typeDecl.Name == "ICorRuntimeHost";
        }

        public void FindRelations()
        {
            foreach (TypeDefinition typeDecl in _typeList)
            {
                FileInfo assemblyInfo = _typeAssemblyInfoList[typeDecl.FullName];
                AnalyseTypeRelations(assemblyInfo, typeDecl);
            }
            UpdateRelationProgress(true);
        }

        private ReaderParameters DetermineAssemblyReaderParameters()
        {
            var resolver = new DefaultAssemblyResolver();

            IDictionary<string, bool> paths = new Dictionary<string, bool>();

            if (FileInfo.Exists)
            {
                if (FileInfo.DirectoryName != null &&
                    paths.ContainsKey(FileInfo.DirectoryName) == false)
                {
                    paths.Add(FileInfo.DirectoryName, true);
                    resolver.AddSearchDirectory(FileInfo.DirectoryName);
                }
            }

            ReaderParameters readerParameters = new ReaderParameters() { AssemblyResolver = resolver };
            return readerParameters;
        }

        private void AnalyseTypeElements(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            try
            {
                RegisterType(assemblyFileInfo, typeDecl);
            }
            catch (Exception e)
            {
                Logger.LogException(
                    $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} failed", e);
            }

            foreach (TypeDefinition nestedType in typeDecl.NestedTypes)
            {
                try
                {
                    RegisterType(assemblyFileInfo, nestedType);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} nestedType={nestedType.Name}",
                        e);
                }
            }
        }

        private static string DetermineType(TypeDefinition typeDecl)
        {
            string type;
            if (typeDecl.IsClass)
            {
                type = typeDecl.IsEnum ? "enum" : "class";
            }
            else if (typeDecl.IsInterface)
            {
                type = "interface";
            }
            else
            {
                type = "?";
            }

            if (typeDecl.HasGenericParameters)
            {
                type = "generic " + type;
            }

            return type;
        }

        private void AnalyseTypeRelations(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            AnalyzeTypeInterfaces(assemblyFileInfo, typeDecl);
            AnalyzeTypeBaseClass(assemblyFileInfo, typeDecl);
            AnalyzeTypeFields(assemblyFileInfo, typeDecl);
            AnalyzeTypeProperties(assemblyFileInfo, typeDecl);
            AnalyseTypeMethods(assemblyFileInfo, typeDecl);
        }

        private void AnalyzeTypeInterfaces(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            foreach (TypeReference interf in typeDecl.Interfaces)
            {
                try
                {
                    string context = "Analyze interfaces of type " + typeDecl.Name;
                    RegisterRelation(interf, typeDecl, "realization", context);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} interface={interf.Name}",
                        e);
                }
            }
        }

        private void AnalyzeTypeBaseClass(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            try
            {
                if (typeDecl.BaseType != null)
                {
                    string context = "Analyze base class of type " + typeDecl.Name;
                    RegisterRelation(typeDecl.BaseType, typeDecl, "generalization", context);
                }
            }
            catch (Exception e)
            {
                Logger.LogException($"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name}", e);
            }
        }

        private void AnalyzeTypeFields(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            foreach (FieldDefinition fieldDecl in typeDecl.Fields)
            {
                try
                {
                    string context = "Analyze fields of type " + typeDecl.Name;
                    RegisterRelation(fieldDecl.FieldType, typeDecl, "field", context);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} field={fieldDecl.Name}",
                        e);
                }
            }
        }

        private void AnalyzeTypeProperties(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            foreach (PropertyDefinition propertyDecl in typeDecl.Properties)
            {
                try
                {
                    string context = "Analyze properties of type " + typeDecl.Name;
                    RegisterRelation(propertyDecl.PropertyType, typeDecl, "property", context);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} property={propertyDecl.Name}",
                        e);
                }
            }
        }

        private void AnalyseTypeMethods(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            foreach (MethodDefinition method in typeDecl.Methods)
            {
                try
                {
                    AnalyzeGenericMethodParameters(assemblyFileInfo, typeDecl, method);
                    AnalyzeMethodParameters(assemblyFileInfo, typeDecl, method);
                    AnalyzeMethodReturnType(assemblyFileInfo, typeDecl, method);
                    AnalyseMethodBody(assemblyFileInfo, typeDecl, method);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        "Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} method={method.Name}",
                        e);
                }
            }
        }

        private void AnalyzeGenericMethodParameters(FileInfo assemblyFileInfo, TypeDefinition typeDecl,
            MethodDefinition method)
        {
            foreach (GenericParameter genericArgument in method.GenericParameters)
            {
                foreach (TypeReference constraint in genericArgument.Constraints)
                {
                    try
                    {
                        string context = "Analyze generic parameters of method " + typeDecl.Name + "::" + method.Name;
                        RegisterRelation(constraint, typeDecl, "parameter", context);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(
                            $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} constraint={constraint.Name}",
                            e);
                    }
                }
            }
        }

        private void AnalyzeMethodParameters(FileInfo assemblyFileInfo, TypeDefinition typeDecl, MethodDefinition method)
        {
            foreach (ParameterDefinition paramDecl in method.Parameters)
            {
                try
                {
                    string context = "Analyze parameters of method " + typeDecl.Name + "::" + method.Name;
                    RegisterRelation(paramDecl.ParameterType, typeDecl, "parameter", context);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} method={method.Name} parameter={paramDecl.Name}",
                        e);
                }
            }
        }

        private void AnalyzeMethodReturnType(FileInfo assemblyFileInfo, TypeDefinition typeDecl, MethodDefinition method)
        {
            TypeReference returnType = method.ReturnType;

            try
            {
                string context = "Analyze return type of method " + typeDecl.Name + "::" + method.Name;
                RegisterRelation(returnType, typeDecl, "return", context);
            }
            catch (Exception e)
            {
                Logger.LogException(
                    $"Analysis failed assemblyFileInfo={assemblyFileInfo.FullName} type={typeDecl.Name} method={method.Name} return={returnType.Name}",
                    e);
            }
        }

        private void AnalyseMethodBody(FileInfo assemblyFileInfo, TypeDefinition typeDecl, MethodDefinition method)
        {
            Mono.Cecil.Cil.MethodBody body = method.Body;

            try
            {
                if (body != null)
                {
                    AnalyzeLocalVariables(assemblyFileInfo, typeDecl, method, body);
                    AnalyzeBodyTypeReferences(typeDecl, method, body);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(
                    $"Analysis failed asssmbly={assemblyFileInfo.FullName} type={typeDecl.Name} method={method.Name}", e);
            }
        }

        private void AnalyzeLocalVariables(FileInfo assemblyFileInfo, TypeDefinition typeDecl, MethodDefinition method,
            Mono.Cecil.Cil.MethodBody body)
        {
            foreach (Mono.Cecil.Cil.VariableDefinition variable in body.Variables)
            {
                try
                {
                    string context = "Analyze local variables of method " + typeDecl.Name + "::" + method.Name;
                    RegisterRelation(variable.VariableType, typeDecl, "local", context);
                }
                catch (Exception e)
                {
                    Logger.LogException(
                        $"Analysis failed assembly={assemblyFileInfo.FullName} type={typeDecl.Name} method={method.Name} variable={variable}",
                        e);
                }
            }
        }

        private void AnalyzeBodyTypeReferences(TypeDefinition typeDecl, MethodDefinition method,
            Mono.Cecil.Cil.MethodBody body)
        {
            int index = 0;

            var instructions = body.Instructions;
            while (index < instructions.Count)
            {
                var i = instructions[index];
                var opCode = i.OpCode;

                switch (opCode.OperandType)
                {
                    case Mono.Cecil.Cil.OperandType.InlineTok:
                    case Mono.Cecil.Cil.OperandType.InlineType:
                    case Mono.Cecil.Cil.OperandType.InlineMethod:
                    case Mono.Cecil.Cil.OperandType.InlineField:
                        {
                            object op = i.Operand;

                            if (op == null)
                            {
                                Logger.LogError("Unexpected null operand in method=" + method.Name);
                            }
                            else
                            {
                                TypeReference t = op as TypeReference;
                                if (t != null)
                                {
                                    string context = "Analyze type references of method " + typeDecl.Name + "::" +
                                                     method.Name;
                                    RegisterRelation(t, typeDecl, "reference", context);
                                }
                                else
                                {
                                    MemberReference m = op as MemberReference;
                                    if (m != null)
                                    {
                                        string context = "Analyze member references of method " + typeDecl.Name + "::" +
                                                         method.Name;
                                        RegisterRelation(m.DeclaringType, typeDecl, "reference", context);
                                    }
                                    else
                                    {
                                        Logger.LogError("Unhandled token type: " + op + " in method = " + method.Name);
                                    }
                                }
                            }
                        }
                        break;
                }

                index++;
            }
        }

        private void RegisterType(FileInfo assemblyFileInfo, TypeDefinition typeDecl)
        {
            string typeName = typeDecl.GetElementType().ToString();
            if (!Ignore(typeName))
            {
                Types.Add(new AssemblyType(typeDecl.GetElementType().ToString(), DetermineType(typeDecl)));
                _typeList.Add(typeDecl);
                _typeAssemblyInfoList[typeDecl.FullName] = assemblyFileInfo;
                UpdateTypeProgress(false);
            }
        }

        private void RegisterRelation(TypeReference providerType, TypeReference consumerType, string type,
            string context)
        {
            if ((providerType != null) && (consumerType != null))
            {
                string consumerName = consumerType.GetElementType().ToString();
                string providerName = providerType.GetElementType().ToString();

                if (!providerType.ContainsGenericParameter &&
                    !Ignore(providerName))
                {
                    Relations.Add(new AssemblyTypeRelation(consumerName, providerName, type));
                    UpdateRelationProgress(false);
                }

                GenericInstanceType providerGenericType = providerType as GenericInstanceType;
                if (providerGenericType != null)
                {
                    foreach (TypeReference providerGenericArgumentType in providerGenericType.GenericArguments)
                    {
                        RegisterRelation(providerGenericArgumentType, consumerType, type, context);
                    }
                }
            }
        }

        private bool Ignore(string providerName)
        {
            bool ignore = false;

            foreach (string ignoredName in _ignoredNames)
            {
                Regex regex = new Regex(ignoredName);
                Match match = regex.Match(providerName);
                if (match.Success)
                {
                    Logger.LogInfo($"Ignored {providerName} due to {ignoredName}");
                    ignore = true;
                }
            }
            return ignore;
        }

        private void UpdateTypeProgress(bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Finding types";
            progressInfo.CurrentItemCount = Types.Count;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "types";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }

        private void UpdateRelationProgress(bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo();
            progressInfo.ActionText = "Finding relations";
            progressInfo.CurrentItemCount = Relations.Count;
            progressInfo.TotalItemCount = 0;
            progressInfo.ItemType = "relations";
            progressInfo.Percentage = null;
            progressInfo.Done = done;
            _progress?.Report(progressInfo);
        }
    }
}

