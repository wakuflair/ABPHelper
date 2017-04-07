using System;
using System.IO;
using System.Linq;
using System.Windows;
using ABPHelper.Models.TemplateModels;
using EnvDTE;
using EnvDTE80;
using RazorEngine.Templating;
using Engine = RazorEngine.Engine;

namespace ABPHelper.Helper
{
    public abstract class HelperBase<TParam>
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly DTE2 Dte;

        protected HelperBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
        }

        public abstract bool CanExecute(TParam parameter);
        public abstract void Execute(TParam parameter);

        protected void CreateAndAddFile(ProjectItem parentItem, string fileName, string content)
        {
            string path = Path.GetTempPath();
            Directory.CreateDirectory(path);
            string file = Path.Combine(path, fileName);
            File.WriteAllText(file, content);
            try
            {
                parentItem.ProjectItems.AddFromFileCopy(file);
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected ProjectItem FindProjectItem(ProjectItem parentItem, string name, string type)
        {
            return FindProjectItem(parentItem.ProjectItems, name, type);
        }

        protected ProjectItem FindProjectItem(ProjectItems parentItems, string name, string type)
        {
            foreach (ProjectItem projectItem in parentItems)
            {
                if (projectItem.Name == name && projectItem.Kind == type) return projectItem;
            }
            return null;
        }
    }
}