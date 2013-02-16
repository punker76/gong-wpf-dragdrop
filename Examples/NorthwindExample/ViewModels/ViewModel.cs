using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NorthwindExample.ViewModels
{
    internal class ViewModel<TDataModel> : INotifyPropertyChanged
    {
        public ViewModel(TDataModel dataModel)
        {
            this.DataModel = dataModel;
        }

        public TDataModel DataModel { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}