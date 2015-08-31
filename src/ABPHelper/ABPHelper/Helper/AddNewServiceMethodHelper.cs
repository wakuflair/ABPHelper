using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using EnvDTE;
using EnvDTE80;

namespace ABPHelper.Helper
{
    public class AddNewServiceMethodHelper : HelperBase
    {
        private const string ErrMessage = "Please run in the class that implements IApplicationService interface.";
        private const string InterfaceName = "Abp.Application.Services.IApplicationService";
        private readonly DTE2 _dte;
        private CodeClass _serviceClass;
        private CodeInterface _serviceInterface;
        private Document _document;

        public AddNewServiceMethodHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dte = ServiceProvider.GetService(typeof (DTE)) as DTE2;
        }

        public override bool CanExecute(IDictionary<string, object> parameter)
        {
            _document = _dte.ActiveDocument;
            if (_document == null || _document.ProjectItem == null || _document.ProjectItem.FileCodeModel == null)
            {
                MessageBox(ErrMessage);
                return false;
            }

            _serviceClass = GetClass(_document.ProjectItem.FileCodeModel.CodeElements);
            if (_serviceClass == null)
            {
                MessageBox(ErrMessage);
                return false;
            }

            _serviceInterface = GetServiceInterface(_serviceClass as CodeElement);
            if (_serviceInterface == null)
            {
                MessageBox(ErrMessage);
                return false;
            }
            return true;
        }

        public override void Execute(IDictionary<string, object> parameter)
        {
            string[] names = (string[]) parameter["names"];
            bool async = (bool) parameter["async"];
            foreach (string name in names)
            {
                try
                {
                    AddMethodToClass(_serviceClass, name, async);
                    AddMethodToInterface(_serviceInterface, name, async);
                    CreateDtoFiles(_document, name);
                }
                catch (Exception e)
                {
                    MessageBox("Generation failed.\r\nMethod name: {0}\r\nException: {1}", MessageBoxButton.OK, MessageBoxImage.Exclamation, name, e.Message);
                }
            }
        }

        private CodeClass GetClass(CodeElements codeElements)
        {
            var elements = codeElements.Cast<CodeElement>().ToList();
            var result = elements.FirstOrDefault(codeElement => codeElement.Kind == vsCMElement.vsCMElementClass) as CodeClass;
            if (result != null)
            {
                return result;
            }
            foreach (var codeElement in elements)
            {
                result = GetClass(codeElement.Children);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private static CodeInterface GetServiceInterface(CodeElement element)
        {
            CodeElements elements;
            if (element.Kind == vsCMElement.vsCMElementClass)
            {
                elements = (element as CodeClass).ImplementedInterfaces;
            }
            else if (element.Kind == vsCMElement.vsCMElementInterface)
            {
                elements = (element as CodeInterface).Bases;
            }
            else
            {
                throw new ArgumentException("The parameter element is not Class nor Interface");
            }
            var baseInterfaces = elements.Cast<CodeElement>()
                .Where(codeElement => codeElement.Kind == vsCMElement.vsCMElementInterface)
                .Cast<CodeInterface>()
                .ToList();

            var result = baseInterfaces.FirstOrDefault(codeInterface => codeInterface.FullName == InterfaceName);
            if (result != null)
            {
                return element as CodeInterface;
            }
            foreach (var baseInterface in baseInterfaces)
            {
                result = GetServiceInterface(baseInterface as CodeElement);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void AddMethodToClass(CodeClass serviceClass, string name, bool async)
        {
            string parameter = string.Format("{0}Input", name);
            var returnName = string.Format(async ? "Task<{0}Output>" : "{0}Output", name);
            var function = serviceClass.AddFunction(name, vsCMFunction.vsCMFunctionFunction, returnName, -1, vsCMAccess.vsCMAccessPublic);
            function.AddParameter("input", parameter);
            if (async)
            {
                function.StartPoint.CreateEditPoint().ReplaceText(6, "public async", (int) vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
            }
            function.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint().ReplaceText(0, "throw new System.NotImplementedException();", (int) vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
        }

        private void AddMethodToInterface(CodeInterface serviceInterface, string name, bool async)
        {
            string parameter = string.Format("{0}Input", name);
            var returnName = string.Format(async ? "Task<{0}Output>" : "{0}Output", name);
            var function = serviceInterface.AddFunction(name, vsCMFunction.vsCMFunctionFunction, returnName, -1);
            function.AddParameter("input", parameter);
        }

        private void CreateDtoFiles(Document document, string name)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            string templateFile = Path.Combine(Path.GetDirectoryName(location), "Templates", "Template.txt");
            string template = File.ReadAllText(templateFile);

            var parentItem = document.ProjectItem.Collection.Parent as ProjectItem;
            var dtoFolder = parentItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault(item => item.Name == "Dto");
            if (dtoFolder == null)
            {
                dtoFolder = parentItem.ProjectItems.AddFolder("Dto");
            }

            string nameSpace = GetNameSpace(document);
            string path = Path.GetTempPath();
            Directory.CreateDirectory(path);
            foreach (var str in new[] {"Input", "Output"})
            {
                string content = string.Format(template, nameSpace, name, str);
                string file = Path.Combine(path, string.Format("{0}{1}.cs", name, str));
                try
                {
                    File.WriteAllText(file, content);
                    dtoFolder.ProjectItems.AddFromFileCopy(file);
                }
                catch (Exception e)
                {
                    MessageBox(e.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                finally
                {
                    File.Delete(file);
                }
            }
        }

        private string GetNameSpace(Document document)
        {
            return document.ProjectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().FullName;
        }
    }
}