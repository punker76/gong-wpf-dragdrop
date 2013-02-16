using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using GongSolutions.Wpf.DragDrop;
using System.Windows;

namespace NorthwindExample.ViewModels
{
    internal class EmployeesTabViewModel : ViewModel<ICollection<EmployeeViewModel>>, IDropTarget
    {
        public EmployeesTabViewModel(IList<EmployeeViewModel> dataModel)
            : base(dataModel)
        {
            this.Employees = new ListCollectionView((IList)dataModel);
            this.Employees.CurrentChanged += (s, e) => this.SubOrdinates.Refresh();

            this.SubOrdinates = new ListCollectionView((IList)dataModel);
            this.SubOrdinates.Filter = this.FilterSubOrdinate;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var targetEmployee = dropInfo.TargetItem as EmployeeViewModel;
            var employees = this.GetEmployees(dropInfo.Data);

            if (targetEmployee != null && !employees.Contains(targetEmployee))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var targetEmployee = (EmployeeViewModel)dropInfo.TargetItem;
            var employees = this.GetEmployees(dropInfo.Data);

            foreach (var employee in employees)
            {
                employee.DataModel.ReportsTo = targetEmployee.DataModel.EmployeeID;
            }

            this.SubOrdinates.Refresh();
        }

        public ICollectionView Employees { get; private set; }
        public ICollectionView SubOrdinates { get; private set; }

        private bool FilterSubOrdinate(object o)
        {
            var selectedEmployee = (EmployeeViewModel)this.Employees.CurrentItem;
            var employee = (EmployeeViewModel)o;

            if (selectedEmployee != null)
            {
                return employee.DataModel.ReportsTo == selectedEmployee.DataModel.EmployeeID;
            }

            return true;
        }

        private IEnumerable<EmployeeViewModel> GetEmployees(object data)
        {
            if (data is EmployeeViewModel)
            {
                return new[] {(EmployeeViewModel)data};
            }
            else if (data is IEnumerable<EmployeeViewModel>)
            {
                return (IEnumerable<EmployeeViewModel>)data;
            }
            else
            {
                return Enumerable.Empty<EmployeeViewModel>();
            }
        }
    }
}