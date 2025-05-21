using ColculationOfUtilityBills.Data;
using ColculationOfUtilityBills.Models;
using ColculationOfUtilityBills.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ColculationOfUtilityBills.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserInputData> Input { get; set; } = [];
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal TotalCost { get; set; }
        public ObservableCollection<Month> Months { get; set; }
        public ICommand ButtonEnterClick { get; set; }
        public ICommand ButtonNotMeteringDeviceClick { get; set; }

        private Month? _selectedMonth;
        public Month SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    OnPropertyChanged();
                    int MonthValue = _selectedMonth?.Value ?? 0;
                }
            }
        }
        public List<Month> GetDate()
        {
            var months = new List<Month>();
            for (int monthNum = 1; monthNum <= 12; monthNum++)
            {
                string monthName = new DateTime(2000, monthNum, 1).ToString("MMMM", new CultureInfo("ru-RU"));
                var month = new Month
                {
                    Value = monthNum,
                    Name = monthName
                };
                months.Add(month);
            }
            return months;
        }

        public void EnterButton()
        {
            using var db = new AppDbContext();
            if (SelectedMonth == null)
            {
                MessageBox.Show("Выберете период!");
                return;
            }

            DateStart = new DateTime(DateTime.Now.Year, SelectedMonth.Value, 1);
            int lastDay = DateTime.DaysInMonth(DateTime.Now.Year, SelectedMonth.Value);
            DateEnd = new DateTime(DateTime.Now.Year, SelectedMonth.Value,lastDay);

            TotalCost = 0m;    
            var serviceLists = new List<ServicesList>();
            foreach (var item in Input)
            {
                if (item.Value < 0)
                {
                    MessageBox.Show($"Некорректное значение поля {item.NameService} ! ! !");
                    return;
                }
                var lMReading = db.ServicesLists.Where(s => s.IdService == item.IdService)
                    .OrderByDescending(s => s.Id)
                    .Select(s => s.LastMeterReading)
                    .FirstOrDefault();
                if (lMReading != 0 && lMReading > item.Value)
                {
                    MessageBox.Show($"некорректные данные в поле {item.NameService}, проверьте прошлые показатели ! ! !");
                    return;
                }
                var serviceList = new ServicesList
                {
                    IdService = item.IdService,
                    Cost = SetMeterReadingCost(db, item.IdService, item.Value),
                    LastMeterReading = item.Value
                };
                TotalCost += serviceList.Cost;
                serviceLists.Add(serviceList);
            }
            var personPeriod = new PersonPeriod
            {
                StartDate = DateStart,
                EndDate = DateEnd,
                TotalCost = TotalCost
            };
            db.PersonPeriods.Add(personPeriod);
            db.SaveChanges();
            MessageBox.Show(db.PersonPeriods.Select(s => s.Id).Where(s=>s== personPeriod.Id).FirstOrDefault().ToString());
            foreach (var serviceList in serviceLists)
            {
                
                serviceList.IdPersonPeriod = personPeriod.Id;
            }
            db.ServicesLists.AddRange(serviceLists);
            db.SaveChanges();
        }

        public void ButtonPersonPeriod()
        {
            var mainViewModel = new MainViewModel();
            PersonPeriodWindow personPeriodW = new PersonPeriodWindow()
            {
                DataContext = mainViewModel,
            };
            personPeriodW.Show();
            Application.Current.MainWindow.Hide();
        }
        public static Service SetService(AppDbContext db, int id)
        {
            var service = db.Services.FirstOrDefault(s => s.Id == id);
            return service is null ?
                throw new InvalidOperationException("Одна или несколько услуг не найдены в базе данных.") : service;
        }

        public static decimal SetMeterReadingCost(AppDbContext db,int id, int currentReading)
        {
            decimal rate = SetService(db, id).Rate;
            var lastReading = db.ServicesLists
                .Where(s => s.IdService == id)
                .OrderByDescending(s => s.Id)
                .Select(s => s.LastMeterReading)
                .FirstOrDefault();

            if (lastReading <= 0)
            {
                return currentReading * rate;
            }

            int cost = currentReading - lastReading;
            if(cost < 0)
                cost = 0;

            return cost * rate;
        }
        public MainViewModel() 
        {
            ButtonEnterClick = new RelayCommand(EnterButton);
            using var db = new AppDbContext();
            DbInitializer.Seed();
            Months = new ObservableCollection<Month>(GetDate());
            Input = new ObservableCollection<UserInputData>(
                db.Services.Select(s => new UserInputData
                {
                    IdService = s.Id,
                    NameService = s.Name
                    
                }).ToList());
            ButtonNotMeteringDeviceClick = new RelayCommand(ButtonPersonPeriod);
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
                       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



    }
}
