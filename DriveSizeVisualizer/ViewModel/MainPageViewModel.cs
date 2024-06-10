using CommunityToolkit.Mvvm.ComponentModel;
using DriveSizeLib.Model;
using DriveSizeLib.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveSizeVisualizer.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private DriveSizeLib.Model.Directory? _directory;
        public ObservableCollection<FileSystemElementUpdate> LogQueue { get; }
        public bool ComputeParallel { get; set; } = false;
        private string _lastLoadingTimeText;
        public string LastLoadingTimeText {
            get
            { 
                return _lastLoadingTimeText;
            }  
            set 
            { 
                _lastLoadingTimeText = value;
                OnPropertyChanged(nameof(LastLoadingTimeText));
            } 
        }
        private List<string> _availableFileTypes;
        public List<string> AvailableFileTypes
        {
            get
            {
                return _availableFileTypes;
            }
            set
            {
                _availableFileTypes = value;
                OnPropertyChanged(nameof(AvailableFileTypes));
            }
        }

        private bool _loadingFinished = true;
        public bool LoadingFinished
        {
            get
            {
                return _loadingFinished;
            }
            set
            {
                if (_loadingFinished != value)
                {
                    _loadingFinished = value;
                    OnPropertyChanged(nameof(LoadingFinished));
                    OnPropertyChanged(nameof(LoadingRunning));
                }
            }
        }
        public bool LoadingRunning { get { return ! LoadingFinished; } }
        
        public bool SortOrder
        {
            get
            {
                return Sort.Item2;
            }
            set
            {
                Sort = new(Sort.Item1, value);
                OnPropertyChanged(nameof(Sort));
            }
        }
        public string SortProperty
        {
            get
            {
                return Sort.Item1;
            }
            set
            {
                Sort = new(value,Sort.Item2);
                OnPropertyChanged(nameof(Sort));
            }
        }

        public Tuple<string,bool> Sort { get; set; }

        public MainPageViewModel()
        {
            LogQueue = new();
            Sort = new("", true);
            Filter = new("","");
        }
        public DriveSizeLib.Model.Directory? Directory
        {
            get
            {
                return _directory;
            }
            set
            {
                if (_directory != value)
                {
                    _directory = value;
                    OnPropertyChanged(nameof(Directory));
                    if (_directory is not null)
                    {
                        var fileTypes = DriveSizeUtils.GetAllFileTypes(_directory);
                        fileTypes.Insert(0,"All");
                        AvailableFileTypes = fileTypes;
                    }
                }
            }
        }
        public string FilterFileType
        {
            get
            {
                return Filter.Item1;
            }
            set
            {
                Filter = new(value, Filter.Item2);
                OnPropertyChanged(nameof(Filter));
                
            }
        }

        public string FilterSearchString
        {
            get
            {
                return Filter.Item2;
            }
            set
            {
                Filter = new(Filter.Item1, value);
                OnPropertyChanged(nameof(Filter));
            }
        }

        public Tuple<string, string> Filter { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        internal void Log(FileSystemElementUpdate update)
        {
            LogQueue.Add(update);
            if (LogQueue.Count %10 == 0)
                OnPropertyChanged(nameof(LogQueue));
        }
    }
}
