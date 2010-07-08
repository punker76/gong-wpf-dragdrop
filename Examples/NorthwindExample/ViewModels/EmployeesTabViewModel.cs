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
    class EmployeesTabViewModel : ViewModel<ICollection<EmployeeViewModel>>, IDropTarget
    {
        public EmployeesTabViewModel(IList<EmployeeViewModel> dataModel)
            : base(dataModel)
        {
            Employees = new ListCollectionView((IList)dataModel);
            Employees.CurrentChanged += (s, e) => SubOrdinates.Refresh();

            SubOrdinates = new ListCollectionView((IList)dataModel);
            SubOrdinates.Filter = FilterSubOrdinate;
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            EmployeeViewModel targetEmployee = dropInfo.TargetItem as EmployeeViewModel;
            IEnumerable<EmployeeViewModel> employees = GetEmployees(dropInfo.Data);

            if (targetEmployee != null && !employees.Contains(targetEmployee))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            EmployeeViewModel targetEmployee = (EmployeeViewModel)dropInfo.TargetItem;
            IEnumerable<EmployeeViewModel> employees = GetEmployees(dropInfo.Data);

            foreach (EmployeeViewModel employee in employees)
            {
                employee.DataModel.ReportsTo = targetEmployee.DataModel.EmployeeID;
            }

            SubOrdinates.Refresh();
        }

        public ICollectionView Employees { get; private set; }
        public ICollectionView SubOrdinates { get; private set; }

        bool FilterSubOrdinate(object o)
        {
            EmployeeViewModel selectedEmployee = (EmployeeViewModel)Employees.CurrentItem;
            EmployeeViewModel employee = (EmployeeViewModel)o;

            if (selectedEmployee != null)
            {
                return employee.DataModel.ReportsTo == selectedEmployee.DataModel.EmployeeID;
            }

            return true;
        }

        IEnumerable<EmployeeViewModel> GetEmployees(object data)
        {
            if (data is EmployeeViewModel)
            {
                return new[] { (EmployeeViewModel)data };
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
