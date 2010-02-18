using System.Collections.ObjectModel;
using NorthwindExample.Models;

namespace NorthwindExample.ViewModels
{
    class ApplicationViewModel : ViewModel<NorthwindDataClassesDataContext>
    {
        public ApplicationViewModel(NorthwindDataClassesDataContext dataModel)
            : base(dataModel)
        {
            m_AllEmployees = new ObservableCollection<EmployeeViewModel>();

            foreach (Employee employee in DataModel.Employees)
            {
                m_AllEmployees.Add(new EmployeeViewModel(employee));
            }

            EmployeesTab = new EmployeesTabViewModel(m_AllEmployees);
        }

        public EmployeesTabViewModel EmployeesTab { get; private set; }

        ObservableCollection<EmployeeViewModel> m_AllEmployees;
    }
}
