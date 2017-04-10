using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using ABPHelper.Helper;
using ABPHelper.Models.HelperModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ABPHelper.ViewModels
{
    public class AddNewServiceMethodViewModel : ViewModelBase
    {
        #region Names 属性

        private string _names;

        /// <summary>
        /// 设置或取得 Names 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public string Names
        {
            get { return _names; }

            set
            {
                if (Set(ref _names, value))
                {
                }
            }
        }

        #endregion

        #region IsAsync 属性

        private bool _isAsync = true;

        /// <summary>
        /// 设置或取得 IsAsync 属性
        /// 更改属性值并引发PropertyChanged事件
        /// </summary>
        public bool IsAsync
        {
            get { return _isAsync; }

            set
            {
                if (Set(ref _isAsync, value))
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
        public RelayCommand GenerateCommand => _generateCommand ?? (_generateCommand = new RelayCommand(ExecuteGenerateCommand, CanExecuteGenerateCommand));

        private void ExecuteGenerateCommand()
        {
            var helper = new AddNewServiceMethodHelper(AbpHelperWindowControl.ServiceProvider);
            var names = Regex.Split(Names, @"\r\n");

            if (Utils.MessageBox("{0} method{1} will be generated. OK?", MessageBoxButton.OKCancel, MessageBoxImage.Question, names.Length, (names.Length > 1 ? "s" : string.Empty)) == MessageBoxResult.Cancel)
            {
                return;
            }

            var parameter = new AddNewServiceMethodModel()
            {
                Names = names,
                IsAsync = IsAsync,
            };

            if (helper.CanExecute(parameter))
            {
                helper.Execute(parameter);
            }

        }

        private bool CanExecuteGenerateCommand()
        {
            return !string.IsNullOrEmpty(Names);
        }

        #endregion
    }
}