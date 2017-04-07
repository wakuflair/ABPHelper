using System;
using System.IO;
using System.Linq;
using System.Windows;
using ABPHelper.Models.HelperModels;
using ABPHelper.Models.TemplateModels;
using EnvDTE;
using RazorEngine;
using RazorEngine.Templating;

namespace ABPHelper.Helper
{
    public class AddNewServiceMethodHelper : HelperBase<AddNewServiceMethodModel>
    {
        private const string ErrMessage = "Please run in the class that implements IApplicationService interface.";
        private const string InterfaceName = "Abp.Application.Services.IApplicationService";
        private CodeClass _serviceClass;
        private CodeInterface _serviceInterface;
        private Document _document;

        public AddNewServiceMethodHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override bool CanExecute(AddNewServiceMethodModel parameter)
        {
            _document = Dte.ActiveDocument;
            if (_document?.ProjectItem?.FileCodeModel == null)
            {
                Utils.MessageBox(ErrMessage);
                return false;
            }

            _serviceClass = GetClass(_document.ProjectItem.FileCodeModel.CodeElements);
            if (_serviceClass == null)
            {
                Utils.MessageBox(ErrMessage);
                return false;
            }

            _serviceInterface = GetServiceInterface(_serviceClass as CodeElement);
            if (_serviceInterface == null)
            {
                Utils.MessageBox(ErrMessage);
                return false;
            }
            return true;
        }

        public override void Execute(AddNewServiceMethodModel parameter)
        {
            foreach (string name in parameter.Names)
            {
                try
                {
                    AddMethodToClass(_serviceClass, name, parameter.IsAsync);
                    AddMethodToInterface(_serviceInterface, name, parameter.IsAsync);
                    CreateDtoFiles(_document, name);
                }
                catch (Exception e)
                {
                    Utils.MessageBox("Generation failed.\r\nMethod name: {0}\r\nException: {1}", MessageBoxButton.OK, MessageBoxImage.Exclamation, name, e.Message);
                }
            }
            Utils.MessageBox("Done!");
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
                throw new ArgumentException("The parameter element is neither Class nor Interface");
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
            string parameter = $"{name}Input";
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
            string parameter = $"{name}Input";
            var returnName = string.Format(async ? "Task<{0}Output>" : "{0}Output", name);
            var function = serviceInterface.AddFunction(name, vsCMFunction.vsCMFunctionFunction, returnName, -1);
            function.AddParameter("input", parameter);
        }

        private void CreateDtoFiles(Document document, string name)
        {
            var parentItem = document.ProjectItem.Collection.Parent as ProjectItem;
            var dtoFolder = parentItem.ProjectItems.Cast<ProjectItem>().FirstOrDefault(item => item.Name == "Dto");
            if (dtoFolder == null)
            {
                dtoFolder = parentItem.ProjectItems.AddFolder("Dto");
            }

            string nameSpace = GetNamespace(document.ProjectItem);
            foreach (var str in new[] {"Input", "Output"})
            {
                var model = new DtoModel() {Namespace = nameSpace, Name = name, InputOrOutput = str};
                string content = Engine.Razor.RunCompile("DtoTemplate", null, model);
                string fileName = $"{name}{str}.cs";
                try
                {
                    CreateAndAddFile(dtoFolder, fileName, content);
                }
                catch (Exception e)
                {
                    Utils.MessageBox(e.Message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        private string GetNamespace(ProjectItem projectItem)
        {
            return projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().FullName;
        }


    }
}