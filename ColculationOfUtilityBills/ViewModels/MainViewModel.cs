using ColculationOfUtilityBills.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColculationOfUtilityBills.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserInputData> Input { get; set; } = [];
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool HasMeteringDevice { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
