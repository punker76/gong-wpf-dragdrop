using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using Showcase.WPF.DragDrop.Models;

namespace Showcase.WPF.DragDrop.ViewModels
{
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
        private SampleData data;
        private ICommand openIssueCommand;
        private ICommand openPullRequestCommand;
        private ICommand openLinkCommand;
        private ICommand filterCollectionCommand;

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
            this.Members.Add(listMember);

            listMember = new ListMember() { Name = "Item 2 with SubItems" };
            listMember.Add(new SubMember() { Name = "SubItem 1" });
            listMember.Add(new SubMember() { Name = "SubItem 2" });
            this.Members.Add(listMember);

            listMember = new ListMember() { Name = "Item 3" };
            this.Members.Add(listMember);

            this.Data = new SampleData();
            this.OpenIssueCommand = new SimpleCommand(issue => { OpenUrlLink($"https://github.com/punker76/gong-wpf-dragdrop/issues/{issue}"); });
            this.OpenPullRequestCommand = new SimpleCommand(pr => { OpenUrlLink($"https://github.com/punker76/gong-wpf-dragdrop/pull/{pr}"); });
            this.OpenLinkCommand = new SimpleCommand(link => { OpenUrlLink(link.ToString()); });
            this.FilterCollectionCommand = new SimpleCommand(isChecked =>
                {
                    var coll = this.Data.FilterCollection1;
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
            get => this.data;
            set
            {
                if (Equals(value, this.data)) return;
                this.data = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand OpenIssueCommand
        {
            get => this.openIssueCommand;
            set
            {
                if (Equals(value, this.openIssueCommand)) return;
                this.openIssueCommand = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand OpenPullRequestCommand
        {
            get => this.openPullRequestCommand;
            set
            {
                if (Equals(value, this.openPullRequestCommand)) return;
                this.openPullRequestCommand = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand OpenLinkCommand
        {
            get => this.openLinkCommand;
            set
            {
                if (Equals(value, this.openLinkCommand)) return;
                this.openLinkCommand = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand FilterCollectionCommand
        {
            get => this.filterCollectionCommand;
            set
            {
                if (Equals(value, this.filterCollectionCommand)) return;
                this.filterCollectionCommand = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable SortDropTargetItems(IEnumerable items)
        {
            return this.SortDragPreviewItems(items);
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

            return allItems;
        }
    }
}