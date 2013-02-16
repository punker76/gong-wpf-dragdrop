using System.Collections.ObjectModel;
using NorthwindExample.Models;

namespace NorthwindExample.ViewModels
{
    internal class ApplicationViewModel : ViewModel<NorthwindDataClassesDataContext>
    {
        public ApplicationViewModel(NorthwindDataClassesDataContext dataModel)
            : base(dataModel)
        {
            this.m_AllEmployees = new ObservableCollection<EmployeeViewModel>();

            foreach (var employee in this.DataModel.Employees)
            {
                this.m_AllEmployees.Add(new EmployeeViewModel(employee));
            }

            this.EmployeesTab = new EmployeesTabViewModel(this.m_AllEmployees);
        }

        public EmployeesTabViewModel EmployeesTab { get; private set; }

        private readonly ObservableCollection<EmployeeViewModel> m_AllEmployees;
    }
}