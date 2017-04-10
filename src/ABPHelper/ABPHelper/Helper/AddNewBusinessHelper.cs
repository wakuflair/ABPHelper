using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ABPHelper.Extensions;
using ABPHelper.Models;
using ABPHelper.Models.HelperModels;
using ABPHelper.Models.TemplateModels;
using EnvDTE;
using RazorEngine;
using RazorEngine.Templating;

namespace ABPHelper.Helper
{
    public class AddNewBusinessHelper : HelperBase<AddNewBusinessModel>
    {
        private Project _appProj;

        private Project _webProj;

        private string _appName;

        public AddNewBusinessHelper(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _appName = Dte.Solution.Properties.Item("Name").Value.ToString();
        }

        public override bool CanExecute(AddNewBusinessModel parameter)
        {
            var solution = Dte.Solution;
            var projects = solution.Projects.OfType<Project>();
            foreach (var project in projects)
            {
                var m = Regex.Match(project.Name, @"(.+)\.Application");
                if (m.Success)
                {
                    _appProj = project;
                }
                if (Regex.IsMatch(project.Name, @"(.+)\.Web"))
                {
                    _webProj = project;
                }
                if (_appProj != null && _webProj != null) break;
            }
            if (_appProj == null)
            {
                Utils.MessageBox("Cannot find the Application project. Please ensure that your are in the ABP solution.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            if (_webProj == null)
            {
                Utils.MessageBox("Cannot find the Web project. Please ensure that your are in the ABP solution.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            return true;
        }

        public override void Execute(AddNewBusinessModel parameter)
        {
            try
            {
                var folder = AddDeepFolder(_appProj.ProjectItems, parameter.ServiceFolder);
                AddDeepFolder(folder.ProjectItems, "Dto");
                CreateServiceFile(parameter, folder);
                CreateServiceInterfaceFile(parameter, folder);
                folder = AddDeepFolder(_webProj.ProjectItems, parameter.ViewFolder);
                CreateViewFiles(parameter, folder);

                Utils.MessageBox("Done!");
            }
            catch (Exception e)
            {
                Utils.MessageBox("Generation failed.\r\nException: {0}", MessageBoxButton.OK, MessageBoxImage.Exclamation, e.Message);
            }
        }

        private void CreateViewFiles(AddNewBusinessModel parameter, ProjectItem folder)
        {
            foreach (var viewFileViewModel in parameter.ViewFiles)
            {
                var model = new ViewFileModel()
                {
                    BusinessName = parameter.BusinessName,
                    Namespace = GetNamespace(parameter.ViewFolder),
                    FileName = viewFileViewModel.FileName,
                    IsPopup = viewFileViewModel.IsPopup,
                    ViewFolder = parameter.ViewFolder,
                    ViewFiles = parameter.ViewFiles,
                };
                foreach (var ext in new[] { ".cshtml", ".js" })
                {
                    var fileName = viewFileViewModel.FileName + ext;
                    if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) continue;
                    string content = Engine.Razor.RunCompile(ext == ".cshtml" ? "CshtmlTemplate" : "JsTemplate", typeof(ViewFileModel), model);
                    CreateAndAddFile(folder, fileName, content);
                }
            }
        }

        private string GetNamespace(string viewFolder)
        {
            return string.Join(".", viewFolder.Split('\\').Select(s => s.LowerFirstChar()));
        }


        private void CreateServiceFile(AddNewBusinessModel parameter, ProjectItem folder)
        {
            var fileName = parameter.ServiceName + ".cs";
            if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) return;
            var model = new ServiceFileModel()
            {
                AppName = _appName,
                Namespace = GetNamespace(parameter),
                InterfaceName = parameter.ServiceInterfaceName,
                ServiceName = parameter.ServiceName,
            };
            string content = Engine.Razor.RunCompile("ServiceFileTemplate", typeof(ServiceFileModel), model);
            CreateAndAddFile(folder, fileName, content);
        }

        private void CreateServiceInterfaceFile(AddNewBusinessModel parameter, ProjectItem folder)
        {
            var fileName = parameter.ServiceInterfaceName + ".cs";
            if (FindProjectItem(folder, fileName, ItemType.PhysicalFile) != null) return;
            var model = new ServiceInterfaceFileModel()
            {
                Namespace = GetNamespace(parameter),
                InterfaceName = parameter.ServiceInterfaceName,
            };
            string content = Engine.Razor.RunCompile("ServiceInterfaceFileTemplate", typeof(ServiceInterfaceFileModel), model);
            CreateAndAddFile(folder, fileName, content);
        }

        private string GetNamespace(AddNewBusinessModel parameter)
        {
            var str = parameter.ServiceFolder.Replace('\\', '.');
            return $"{_appName}.{str}";
        }

        private ProjectItem AddDeepFolder(ProjectItems parentItems, string deepFolder)
        {
            ProjectItem addedFolder = null;
            foreach (var folder in deepFolder.Split('\\'))
            {
                var projectItem = FindProjectItem(parentItems, folder, ItemType.PhysicalFolder);
                addedFolder = projectItem ?? parentItems.AddFolder(folder);
                parentItems = addedFolder.ProjectItems;
            }
            return addedFolder;
        }
    }
}