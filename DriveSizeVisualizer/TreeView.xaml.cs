
using DriveSizeLib.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace DriveSizeVisualizer
{
    public partial class TreeView : ContentView, INotifyPropertyChanged
    {
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(nameof(Node), typeof(DriveSizeLib.Model.Directory), typeof(TreeView), default(DriveSizeLib.Model.Directory),
            propertyChanged:(b, o, n) =>
            {
                if (b is TreeView tV)
                {
                    if (n is DriveSizeLib.Model.Directory d)
                    {
                        tV.Node = d;
                        tV.UpdateChildComponents();
                    }
                }
            });
        public static readonly BindableProperty SortProperty = BindableProperty.Create(nameof(Sort), typeof(Tuple<string, bool>), typeof(TreeView), default(Tuple<string, bool>),
            propertyChanged: (b, o, n) =>
            {
                if (b is TreeView tV)
                {
                    if (n is Tuple<string, bool> sort)
                    {
                        tV.Sort = sort;
                        tV.UpdateChildComponents();
                    }
                }
            });
        public static readonly BindableProperty FilterProperty = BindableProperty.Create(nameof(Filter), typeof(Tuple<string, string>), typeof(TreeView), default(Tuple<string, string>),
            propertyChanged: (b, o, n) =>
            {
                if (b is TreeView tV)
                {
                    if (n is Tuple<string, string> filter)
                    {
                        tV.Filter = filter;
                        tV.UpdateChildComponents();
                    }
                }
            });

        private void UpdateChildComponents()
        {      
            childLayout.Children.Clear();
            bool filter(FileSystemElement element)
            {
                if (element is DriveSizeLib.Model.Directory dir)
                {
                    return dir.Children.Any(it => filter(it));
                }
                else
                {
                    var file = element as DriveSizeLib.Model.File;
                    return ((string.IsNullOrEmpty(Filter.Item1)||Filter.Item1 == "All" )|| Filter.Item1 == file.Type)
                        && (string.IsNullOrEmpty(Filter.Item2) || file.Name.StartsWith(Filter.Item2,StringComparison.CurrentCultureIgnoreCase));
                }
            }

            if (Node != null) 
            {
                foreach (var child in GetOrderedChildren(Sort))
                {
                    if(child is DriveSizeLib.Model.Directory dir)
                    {
                        childLayout.Add(
                            new TreeView(this.Sort,this.Filter)
                            { Node = dir,
                            IsVisible = filter(dir)});
                    }
                    else if (child is DriveSizeLib.Model.File file)
                    {
                        childLayout.Add(
                            new FileView()
                            {
                                File = file,
                                IsVisible = filter(file)
                            });
                    }
                }
            }
        }

            private IQueryable<FileSystemElement> GetOrderedChildren(Tuple<string,bool>filter)
            {
                var source = Node.Children.AsQueryable();
                var expression = source.Expression;
                ParameterExpression parameter = Expression.Parameter(typeof(FileSystemElement));
                var selector = Expression.Property(parameter, filter.Item1);
                var method = filter.Item2 ? "OrderBy":"OrderByDescending";
                expression = Expression.Call(typeof(Queryable), method,
                    new[] { source.ElementType, selector.Type },
                    expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                return source.Provider.CreateQuery<FileSystemElement>(expression);
            }

        public DriveSizeLib.Model.Directory Node
        {
            get => (DriveSizeLib.Model.Directory)GetValue(TreeView.NodeProperty);
            set => SetValue(TreeView.NodeProperty, value);
        }
        public Tuple<string,bool> Sort
        {
            get => (Tuple<string, bool>) GetValue(TreeView.SortProperty);
            set => SetValue(TreeView.SortProperty, value);
        }
        public Tuple<string, string> Filter
        {
            get => (Tuple<string, string>)GetValue(TreeView.FilterProperty);
            set => SetValue(TreeView.FilterProperty, value);
        }
        public TreeView()
        {
            InitializeComponent();
            label.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        ToggleChild();
                    })
                }
            );
        }
        public TreeView(Tuple<string, bool> sort, Tuple<string, string> filter)
        {
            InitializeComponent();
            Sort = sort;
            Filter = filter;
            label.GestureRecognizers.Add(
                new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        ToggleChild();
                    })
                }
            );
        }
        private void ToggleChild()
        {
            childLayout.IsVisible = !childLayout.IsVisible;
        }
    }
}