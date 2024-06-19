using CommunityToolkit.Mvvm.ComponentModel;
using DriveSizeLib.Model;
using DriveSizeLib.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TreeView.Maui.Core;

namespace DriveSizeVisualizer.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<FileSystemElementTreeWrapper> _directories;
        private FileSystemElement _sourceOfTruth;
        private Stack<DriveSizeLib.Model.Directory> History;
        public ObservableCollection<FileSystemElementUpdate> LogQueue { get; }
        private DateTime? _lastLogTime = null;
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
                if (_directories.Count > 0)
                {
                    _directories[0].UpdateChildren();
                    OnPropertyChanged(nameof(Directories));
                }
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
                if(_directories.Count > 0)
                {
                    _directories[0].UpdateChildren();
                    OnPropertyChanged(nameof(Directories));
                }

            }
        }

        public Tuple<string,bool> Sort { get; set; }

        public MainPageViewModel()
        {
            LogQueue = new();
            Sort = new("", true);
            Filter = new("","");
            _directories = new();
            _renderDepth = 3;
            History = new();
        }
        public ObservableCollection<FileSystemElementTreeWrapper> Directories
        {
            get
            {
                return _directories;
            }
            set
            {
                _directories =  value;
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
                OnPropertyChanged(nameof(FilterFileType));
                OnPropertyChanged(nameof(Filter));
                if (_directories.Count > 0)
                {
                    _directories[0].UpdateChildren();
                    OnPropertyChanged(nameof(Directories));
                }

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
                if (_directories.Count > 0)
                {
                    _directories[0].UpdateChildren();
                    OnPropertyChanged(nameof(Directories));
                }
            }
        }

        public Tuple<string, string> Filter { get; set; }
        private int _renderDepth;


        public int RenderDepth 
        {
            get
            {
                return _renderDepth;
            }
            set
            {
                _renderDepth = value;
                OnPropertyChanged(nameof(RenderDepth));
                Render();
            }
        }
        public bool ShowTree
        {
            get
            {
                return _directories.Count > 0;
            }
        }
        private void Render()
        {
            _directories.Clear();
            //CutOff(_sourceOfTruth, out renderDir);
            
            if (_sourceOfTruth is not null)
            {
                var wrapped = FileSystemElementTreeWrapper.FromElement(_sourceOfTruth, this);
                _directories.Add(wrapped);
                if (_sourceOfTruth is not null)
                {
                    var fileTypes = DriveSizeUtils.GetAllFileTypes(_sourceOfTruth);
                    fileTypes.Insert(0, "All");
                    AvailableFileTypes = fileTypes;
                }
                OnPropertyChanged(nameof(this.Directories));
                OnPropertyChanged(nameof(this.ShowTree));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void ReRender()
        {
            OnPropertyChanged(nameof(Directories));
        }
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        internal void Log(FileSystemElementUpdate update)
        {
            _lastLogTime ??= DateTime.Now;
            lock (this.LogQueue)
            {
                LogQueue.Add(update);
                if (LogQueue.Count == 101)
                {
                    //LogQueue.RemoveAt(0);
                }
            }
            if ((DateTime.Now - _lastLogTime).Value.TotalSeconds > 1)
            {
                OnPropertyChanged(nameof(LogQueue));
                _lastLogTime = DateTime.Now;
            }
        }
        public void ViewSubDirectory(DriveSizeLib.Model.Directory? directory)
        {
            History.Push(_sourceOfTruth as DriveSizeLib.Model.Directory);
            AddDirectory(directory);
        }
        public void GoBack()
        {
            if (History.Count > 0)
            {
                var last = History.Pop();
                AddDirectory(last);
            }
        }
        internal void AddDirectory(DriveSizeLib.Model.Directory? dir)
        {
            if (dir is not null)
            {
                _sourceOfTruth = dir;
                Render();
            }
        }

        private void CutOff(DriveSizeLib.Model.FileSystemElement elem,out DriveSizeLib.Model.FileSystemElement? newElement, int depth = -1)
        {
            if(depth == RenderDepth)
            {
                newElement = null;
                return;
            }
            else
            {
                
                    if (elem is DriveSizeLib.Model.Directory) newElement = new DriveSizeLib.Model.Directory(elem.Path, elem.Parent);
                    else if(elem is DriveSizeLib.Model.File file)
                    {
                        newElement = new DriveSizeLib.Model.File(file.Path, file.Parent as DriveSizeLib.Model.Directory,file.Type,file.Size);
                        return;
                    }
                    else { newElement = null; return; }
                
                depth++;
                if (elem is DriveSizeLib.Model.Directory dir)
                {
                    var newChildren = new List<FileSystemElement>();

                        for (int i = 0; i < dir.Children.Count; i++)
                        {
                            FileSystemElement child;
                            CutOff(dir.Children[i],out child, depth);
                            if (child is not null)
                                newChildren.Add(child);

                        }
                    
                    (newElement as DriveSizeLib.Model.Directory).SetChildren(newChildren);
                }
                
            }
        }
    }
}
