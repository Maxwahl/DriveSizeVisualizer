using DriveSizeLib.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveSizeVisualizer.ViewModel
{
    public class FileSystemElementTreeWrapper
    {
        public FileSystemElement? Element { get; init; }

        public string Name => Element?.DisplayString ??"";
        public ObservableCollection<FileSystemElementTreeWrapper> Children
        {
            get; set;
        }
        public int Depth { get; set; }
        public FileSystemElementTreeWrapper(FileSystemElement element,MainPageViewModel viewModel,int depth,bool extended)
        {
            Element = element;
            MainPageViewModel = viewModel;
            ViewSubDirectoryCommand = new Command<FileSystemElementTreeWrapper>((wr) =>
            {
                wr.MainPageViewModel.ViewSubDirectory(wr.Element as DriveSizeLib.Model.Directory);
            });
            ExpandCommand = new Command<FileSystemElementTreeWrapper>(
                (wr) =>
                {
                    wr.IsExtended = !wr.IsExtended;
                    wr.MainPageViewModel.ReRender();
                });
            Children = new ObservableCollection<FileSystemElementTreeWrapper>();
            IsExtended = extended;
            Depth = depth;
            UpdateChildren();
        }
        internal static FileSystemElementTreeWrapper FromElement(FileSystemElement? element,MainPageViewModel viewModel,int depth =-1,bool extended =true)
        {
            if (depth == viewModel.RenderDepth) 
                return null;
            return new FileSystemElementTreeWrapper(element, viewModel,depth+1,extended);
        }
        public ICommand ViewSubDirectoryCommand { get; private set; }
        public ICommand ExpandCommand { get; private set; }

        public MainPageViewModel MainPageViewModel { get; init;}
        public bool ButtonVisible
        {
            get
            {
                return Depth != 0 && Element is DriveSizeLib.Model.Directory;
            }
        }


        private IQueryable<FileSystemElement> GetOrderedChildren()
        {
            var filter = MainPageViewModel.Filter;
            var sort = MainPageViewModel.Sort;
            bool applyFilter(FileSystemElement element)
            {
                if (element is DriveSizeLib.Model.Directory dir)
                {
                    if (string.IsNullOrEmpty(filter.Item1) && string.IsNullOrEmpty(filter.Item2))
                        return true;
                    return dir.Children.Any(it => applyFilter(it));
                }
                else
                {
                    var file = element as DriveSizeLib.Model.File;
                    return ((string.IsNullOrEmpty(filter.Item1) || filter.Item1 == "All") || filter.Item1 == file.Type)
                        && (string.IsNullOrEmpty(filter.Item2) || file.Name.StartsWith(filter.Item2, StringComparison.CurrentCultureIgnoreCase));
                }
            }


            var source = (Element as DriveSizeLib.Model.Directory).Children.Where(it => applyFilter(it)).AsQueryable();
            var expression = source.Expression;
            ParameterExpression parameter = Expression.Parameter(typeof(FileSystemElement));
            var selector = Expression.Property(parameter, sort.Item1);
            var method = sort.Item2 ? "OrderBy" : "OrderByDescending";
            expression = Expression.Call(typeof(Queryable), method,
                new[] { source.ElementType, selector.Type },
                expression, Expression.Quote(Expression.Lambda(selector, parameter)));
            return source.Provider.CreateQuery<FileSystemElement>(expression);
        }

        internal void UpdateChildren()
        {
            //Dictionary<FileSystemElement, bool> extended = Children.Select(it => new { Key = it.Element, Value = it.IsExtended }).ToDictionary(it => it.Key, it => it.Value);
            
            Children.Clear();
            if (Element is DriveSizeLib.Model.Directory dir)
            {
                foreach (var child in GetOrderedChildren())
                {
                    bool extend = true;
                    //if (extended.ContainsKey(child)) extend = extended[child];
                    var wrapperChild = FromElement(child, MainPageViewModel, Depth, extend);
                    if (wrapperChild is not null)
                        Children.Add(wrapperChild);
                }
            }
        }
        public bool IsExtended { get; set; }
    }
}
