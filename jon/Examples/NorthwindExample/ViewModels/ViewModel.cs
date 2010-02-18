using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NorthwindExample.ViewModels
{
    class ViewModel<TDataModel> : INotifyPropertyChanged
    {
        public ViewModel(TDataModel dataModel)
        {
            DataModel = dataModel;
        }

        public TDataModel DataModel { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
