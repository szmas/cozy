﻿using FileTester.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using CozyAnywhere.Plugin.WinFile;
using CozyAnywhere.Plugin.WinFile.Model;

namespace FileTester.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<WinFileModel> _FileList = new ObservableCollection<WinFileModel>();

        public ObservableCollection<WinFileModel> FileList
        {
            get
            {
                return _FileList;
            }
            set
            {
                Set(ref _FileList, value, "FileList");
            }
        }

        private WinFileModel _SelectedItem = null;

        public WinFileModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                Set(ref _SelectedItem, value, "SelectedItem");
            }
        }

        private string _CurrPath;

        public string CurrPath
        {
            get
            {
                return _CurrPath;
            }
            set
            {
                Set(ref _CurrPath, value, "CurrPath");
            }
        }

        private ICommand _FileDeleteCommand;

        public ICommand FileDeleteCommand
        {
            get
            {
                return _FileDeleteCommand = _FileDeleteCommand ?? new DelegateCommand(
                    (x) =>
                    {
                        if (SelectedItem != null)
                        {
                            var defer = SelectedItem;
                            FileUtil.FileDelete(defer.Name);
                            UpdateFileList();
                        }
                    });
            }
        }

        public MainWindowViewModel()
        {
            PropertyChanged += OnProptrtyChanged;
            CurrPath        = @".\";
        }

        public void OnProptrtyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrPath":
                    UpdateFileList();
                    break;

                default:
                    break;
            }
        }

        private void UpdateFileList()
        {
            ObservableCollection<WinFileModel> newList = new ObservableCollection<WinFileModel>();
            var path = CurrPath.Trim();
            if (path.EndsWith(@"\"))
            {
                path += '*';
            }
            else
            {
                if (!path.EndsWith(@"*"))
                {
                    path += @"\*";
                }
            }
            FileUtil.FileEnum(path, (x, b) =>
            {
                var str = Marshal.PtrToStringAuto(x);
                newList.Add(
                    new WinFileModel
                    {
                        Name        = CurrPath + str,
                        IsFolder    = b,
                    });
                return false;
            });
            FileList = newList;
        }
    }
}