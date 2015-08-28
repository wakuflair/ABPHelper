using System;
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
        private const string ErrMessage = "请在实现了IApplicationService接口的类中使用此功能";
        private const string InterfaceName = "Abp.Application.Services.IApplicationService";
        private readonly DTE2 _dte;

        public AddNewServiceMethodHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dte = ServiceProvider.GetService(typeof (DTE)) as DTE2;
        }

        public override void Execute()
        {
            var document = _dte.ActiveDocument;
            if (document == null || document.ProjectItem == null || document.ProjectItem.FileCodeModel == null)
            {
                MessageBox(ErrMessage);
                return;
            }

            var serviceClass = GetClass(document.ProjectItem.FileCodeModel.CodeElements);
            if (serviceClass == null)
            {
                MessageBox(ErrMessage);
                return;
            }

            var serviceInterface = GetServiceInterface(serviceClass as CodeElement);
            if (serviceInterface == null)
            {
                MessageBox(ErrMessage);
                return;
            }

            string name = "TestMethod";
            AddMethodToClass(serviceClass, name);
            AddMethodToInterface(serviceInterface, name);
            CreateDtoFiles(document, name);
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

        private void AddMethodToClass(CodeClass serviceClass, string name)
        {
            string parameter = string.Format("{0}Input", name);
            string returnName = string.Format("Task<{0}Output>", name);
            var function = serviceClass.AddFunction(name, vsCMFunction.vsCMFunctionFunction, returnName, -1, vsCMAccess.vsCMAccessPublic);
            function.AddParameter("input", parameter);
            function.StartPoint.CreateEditPoint().ReplaceText(6, "public async", (int) vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
            function.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint().ReplaceText(0, "throw new System.NotImplementedException();", (int) vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
        }

        private void AddMethodToInterface(CodeInterface serviceInterface, string name)
        {
            string parameter = string.Format("{0}Input", name);
            string returnName = string.Format("Task<{0}Output>", name);
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