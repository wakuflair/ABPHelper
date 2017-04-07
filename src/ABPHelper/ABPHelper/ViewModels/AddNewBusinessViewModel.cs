using System.Collections.Generic;
using System.Windows;
using ABPHelper.Helper;
using ABPHelper.Models;
using ABPHelper.Models.HelperModels;
using ABPHelper.Models.TemplateModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ABPHelper.ViewModels
{
    public class AddNewBusinessViewModel : ViewModelBase
    {
        public AddNewBusinessViewModel()
        {
#if DEBUG
            BusinessName = "Contract";
#endif
        }

        #region BusinessName 属性

        private string _businessName;

        /// <summary>
        /// 设置或取得 BusinessName 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public string BusinessName
        {
            get { return _businessName; }

            set
            {
                if (Set(ref _businessName, value))
                {
                    ServiceFolder = BusinessName + "s";
                    ServiceInterfaceName = $"I{BusinessName}AppService";
                    ServiceName = $"{BusinessName}AppService";
                }
            }
        }

        #endregion

        #region ServiceFolder 属性

        private string _serviceFolder;

        /// <summary>
        /// 设置或取得 ServiceFolder 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public string ServiceFolder
        {
            get { return _serviceFolder; }

            set
            {
                if (Set(ref _serviceFolder, value))
                {
                }
            }
        }

        #endregion

        #region ServiceInterfaceName 属性

        private string _serviceInterfaceName;

        /// <summary>
        /// 设置或取得 ServiceInterfaceName 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public string ServiceInterfaceName
        {
            get { return _serviceInterfaceName; }

            set
            {
                if (Set(ref _serviceInterfaceName, value))
                {
                }
            }
        }

        #endregion

        #region ServiceName 属性

        private string _serviceName;

        /// <summary>
        /// 设置或取得 ServiceName 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }

            set
            {
                if (Set(ref _serviceName, value))
                {
                }
            }
        }

        #endregion

        #region GenerateCommand 命令 

        private RelayCommand _generateCommand;

        /// <summary>
        /// 取得 GenerateCommand。
        /// </summary>
        public RelayCommand GenerateCommand => _generateCommand ?? (_generateCommand = new RelayCommand(
            ExecuteGenerateCommand,
            CanExecuteGenerateCommand));

        private void ExecuteGenerateCommand()
        {
            if (Utils.MessageBox("Are you sure to generate?", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                return;
            }

            var helper = new AddNewBusinessHelper(AbpHelperWindowControl.ServiceProvider);
            var parameter = new AddNewBusinessModel()
            {
                BusinessName = BusinessName,
                ServiceFolder = ServiceFolder,
                ServiceInterfaceName = ServiceInterfaceName,
                ServiceName = ServiceName,
            };
            if (helper.CanExecute(parameter))
            {
                helper.Execute(parameter);
            }
        }

        private bool CanExecuteGenerateCommand()
        {
            return !string.IsNullOrEmpty(BusinessName) &&
                   !string.IsNullOrEmpty(ServiceFolder) &&
                   !string.IsNullOrEmpty(ServiceInterfaceName) &&
                   !string.IsNullOrEmpty(ServiceName);
        }

        #endregion

    }
}