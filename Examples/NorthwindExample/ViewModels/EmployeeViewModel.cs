using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NorthwindExample.Models;

namespace NorthwindExample.ViewModels
{
  internal class EmployeeViewModel : ViewModel<Employee>
  {
    public EmployeeViewModel(Employee dataModel)
      : base(dataModel)
    {
    }

    public string FirstName
    {
      get { return this.DataModel.FirstName; }
    }

    public string FullName
    {
      get { return this.FirstName + " " + this.LastName; }
    }

    public string LastName
    {
      get { return this.DataModel.LastName; }
    }
  }
}