using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Input;
using Showcase.WPF.DragDrop.Models;

namespace Showcase.WPF.DragDrop.ViewModels
{
    using GongSolutions.Wpf.DragDrop;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class ListMember : List<SubMember>
    {
        public string Name { get; set; }
    }

    public class SubMember
    {
        public string Name { get; set; }
    }

    public class MainViewModel : ViewModelBase, IDragPreviewItemsSorter, IDropTargetItemsSorter
    {
        private SampleData _data;
        private ICommand _openIssueCommand;
        private ICommand _openPullRequestCommand;
        private ICommand _openLinkCommand;
        private ICommand _filterCollectionCommand;

        public ObservableCollection<ListMember> Members { get; set; } = new ObservableCollection<ListMember>();

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            var listMember = new ListMember() { Name = "Item 1" };
            Members.Add(listMember);

            listMember = new ListMember() { Name = "Item 2 with SubItems" };
            listMember.Add(new SubMember() { Name = "SubItem 1" });
            listMember.Add(new SubMember() { Name = "SubItem 2" });
            Members.Add(listMember);

            listMember = new ListMember() { Name = "Item 3" };
            Members.Add(listMember);

            this.Data = new SampleData();
            this.OpenIssueCommand = new SimpleCommand(issue =>
            {
                OpenUrlLink($"https://github.com/punker76/gong-wpf-dragdrop/issues/{issue}");
            });
            this.OpenPullRequestCommand = new SimpleCommand(pr => { OpenUrlLink($"https://github.com/punker76/gong-wpf-dragdrop/pull/{pr}"); });
            this.OpenLinkCommand = new SimpleCommand(link => { OpenUrlLink(link.ToString()); });
            this.FilterCollectionCommand = new SimpleCommand(isChecked =>
                {
                    var coll = Data.FilterCollection1;
                    var collView = CollectionViewSource.GetDefaultView(coll);
                    collView.Filter += o =>
                        {
                            if (!(isChecked as bool?).GetValueOrDefault())
                            {
                                return true;
                            }
                            var itemModel = (ItemModel)o;
                            var number = itemModel.Index;
                            return (number & 0x01) == 0;
                        };
                });

            static void OpenUrlLink(string link) => Process.Start(new ProcessStartInfo
            {
                FileName = link ?? throw new System.ArgumentNullException(nameof(link)),
                // UseShellExecute is default to false on .NET Core while true on .NET Framework.
                // Only this value is set to true, the url link can be opened.
                UseShellExecute = true,
            });
        }

        public SampleData Data
        {
            get { return _data; }
            set
            {
                if (Equals(value, _data)) return;
                _data = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenIssueCommand
        {
            get { return _openIssueCommand; }
            set
            {
                if (Equals(value, _openIssueCommand)) return;
                _openIssueCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenPullRequestCommand
        {
            get { return _openPullRequestCommand; }
            set
            {
                if (Equals(value, _openPullRequestCommand)) return;
                _openPullRequestCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenLinkCommand
        {
            get { return _openLinkCommand; }
            set
            {
                if (Equals(value, _openLinkCommand)) return;
                _openLinkCommand = value;
                OnPropertyChanged();
            }
        }

        public ICommand FilterCollectionCommand
        {
            get { return _filterCollectionCommand; }
            set
            {
                if (Equals(value, _filterCollectionCommand)) return;
                _filterCollectionCommand = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable SortDropTargetItems(IEnumerable items)
        {
            return SortDragPreviewItems(items);
        }

        public IEnumerable SortDragPreviewItems(IEnumerable items)
        {
            var allItems = items.Cast<object>().ToList();
            if (allItems.Count > 0)
            {
                if (allItems[0] is ItemModel)
                {
                    return allItems.OrderBy(x => ((ItemModel)x).Index);
                }
            }
            return items;
        }
    }
}