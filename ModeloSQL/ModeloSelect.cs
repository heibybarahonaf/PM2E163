using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2E163.ModeloSQL
{
    public class ModeloSelect: ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<ModeloSQL.Sitios> items;
        public ObservableCollection<ModeloSQL.Sitios> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        public ModeloSelect(IEnumerable<ModeloSQL.Sitios> items)
        {
            Items = new ObservableCollection<ModeloSQL.Sitios>(items);
            BindingContext = this;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
