using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NorthwindExample.Models;

namespace NorthwindExample.ViewModels
{
    class EmployeeViewModel : ViewModel<Employee>
    {
        public EmployeeViewModel(Employee dataModel)
            : base(dataModel)
        {
        }

        public string FirstName
        {
            get { return DataModel.FirstName; }
        }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        public string LastName
        {
            get { return DataModel.LastName; }
        }
    }
}
