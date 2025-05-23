using ColculationOfUtilityBills.Data;
using ColculationOfUtilityBills.Models;
using ColculationOfUtilityBills.Views;
using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Controls;
using System.Windows.Input;

namespace ColculationOfUtilityBills.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public bool HasMeteringDevice = true;
        public bool SavePersonPeriod = false;
        public PersonPeriod PersonPeriodData { get; set; }
        private ObservableCollection<UserInputData> _input;
        public ObservableCollection<UserInputData> Input
        {
            get => _input;
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PersonPeriod> _personPeriods;
        public ObservableCollection<PersonPeriod> PersonPeriods
        {
            get => _personPeriods;
            set
            {
                _personPeriods = value;
                OnPropertyChanged();
            }
        }
        private DateTime? _dateStartPicker;
        public DateTime? DateStartPicker
        {
            get => _dateStartPicker;
            set
            {
                if (_dateStartPicker != value)
                {
                    _dateStartPicker = value;
                    OnPropertyChanged(nameof(DateStartPicker));
                }
            }
        }

        private DateTime? _dateEndPicker;
        public DateTime? DateEndPicker
        {
            get => _dateEndPicker;
            set
            {
                if (_dateEndPicker != value)
                {
                    _dateEndPicker = value;
                    OnPropertyChanged(nameof(DateEndPicker));
                }
            }
        }
        
        public int PersonsCount { get; set; }
        private ObservableCollection<ServiceListCosts>? _servicesListW;
        public ObservableCollection<ServiceListCosts>? ServicesListW
        {
            get => _servicesListW;
            set
            {
                _servicesListW = value;
                OnPropertyChanged(nameof(ServicesListW));
            }
        }
        public List<ServicesList> serviceLists = new List<ServicesList>();
       
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal TotalCost { get; set; }
        public ObservableCollection<Month> Months { get; set; }
        public ICommand ButtonEnterClick { get; set; }
        public ICommand ButtonNotMeteringDeviceClick { get; set; }
        public ICommand ButtonSave {  get; set; }
        public ICommand ButtonBack { get; set; }
        public ICommand ButtonHistoryClick { get; set; }
        public ICommand ButtonHistoryBack { get; set; }
        public ICommand ButtonMainWindow {  get; set; }
        public bool DateEnabled { get; set; } = false;
        public DateTime DisplayDateStart { get; set; }
        public DateTime DisplayDateEnd { get; set; } 
        public Action? RequestHideHistoryCostWindow { get; set; }
        public Action? RequestShowHistoryCostWindow { get; set; }
        public Action? RequestHideMainWindow { get; set; }
        public Action? RequestHideServiceListWindow { get; set; }
        public Action? RequestShowMainWindow { get; set; }
        public Action? RequestShowServiceListWindow { get; set; }
        public Action? RequestShowPersonPeriodWindow { get; set; }
        public Action? RequestHidePersonPeriodWindow { get; set; }

        private Month? _selectedMonth;
        public Month SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    
                    int MonthValue = _selectedMonth?.Value ?? 0;
                    DisplayDateStart = new DateTime(DateTime.Now.Year, MonthValue, 1);
                    DisplayDateEnd = new DateTime(DateTime.Now.Year, MonthValue, DateTime.DaysInMonth(DateTime.Now.Year, MonthValue));
                    DateStartPicker = DisplayDateStart;
                    DateEndPicker = DisplayDateEnd;
                    DateEnabled = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DateEnabled));
                    OnPropertyChanged(nameof(DisplayDateStart));
                    OnPropertyChanged(nameof(DisplayDateEnd));
                    OnPropertyChanged(nameof(DateStartPicker));
                    OnPropertyChanged(nameof(DateEndPicker));
                }
            }
        }
        public List<Month> GetDate()
        {
            var months = new List<Month>();
            for (int monthNum = 1; monthNum <= DateTime.Now.Month-1; monthNum++)
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
            TotalCost = 0;
            using var db = new AppDbContext();
            if (HasMeteringDevice == false)
            {
                if(DateStartPicker > DateEndPicker || DateEndPicker is null || DateStartPicker is null)
                {
                    MessageBox.Show("Некорректный период ! ! !");
                    return;
                }
                if(PersonsCount <= 0)
                {
                    MessageBox.Show("Количество людей должно быть больше 0 ! ! !");
                    return;
                }
                DateStart = (DateTime)DateStartPicker;
                DateEnd = (DateTime)DateEndPicker;
                var days = DateEnd - DateStart;
                var lastDay = DateTime.DaysInMonth(DateEnd.Year, DateEnd.Month);
                TotalCost = 0m;
                serviceLists = new List<ServicesList>();
                decimal HotWaterStandart = 0m;
                foreach (var item in Input)
                {
                    
                    var service = db.Services.First(s => s.Id == item.IdService);
                    var cost = service.Standart * PersonsCount * service.Rate;
                    
                    if (item.IdService == 5)
                        HotWaterStandart = service.Standart;
                    if (item.IdService == 6)
                        cost = service.Standart * HotWaterStandart*PersonsCount * service.Rate;
                    if (lastDay - (days.Days + 1) > 0)
                    {
                        cost = cost / lastDay*(days.Days + 1);
                    }
                    var serviceList = new ServicesList
                    {
                        IdService = item.IdService,
                        Cost = cost,
                        LastMeterReading = 0,
                        Service = service,
                    };
                    TotalCost += serviceList.Cost;
                    serviceLists.Add(serviceList);

                }
                
            }
            else
            {
                if (SelectedMonth == null)
                {
                    MessageBox.Show("Выберете период!");
                    return;
                }
                decimal HotWaterStandart = 0m;
                DateStart = new DateTime(DateTime.Now.Year, SelectedMonth.Value, 1);
                int lastDay = DateTime.DaysInMonth(DateTime.Now.Year, SelectedMonth.Value);
                DateEnd = new DateTime(DateTime.Now.Year, SelectedMonth.Value, lastDay);

                TotalCost = 0m;
                serviceLists = new List<ServicesList>();
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
                        MessageBox.Show($"некорректные данные в поле {item.NameService}, сверьте прошлые показатели ! ! !");
                        return;
                    }
                    var service = db.Services.First(s => s.Id == item.IdService);
                    var cost = SetMeterReadingCost(db, item.IdService, item.Value);
                    var HotWaterValue = 0m;
                    if (item.IdService == 5)
                    {
                        HotWaterStandart = service.Standart;
                        HotWaterValue = item.Value;
                    }
                        
                    if (item.IdService == 6)
                        cost = service.Standart * HotWaterStandart *HotWaterValue* service.Rate;
                    var serviceList = new ServicesList
                    {
                        IdService = item.IdService,
                        Cost = cost,
                        LastMeterReading = item.Value,
                        Service = service
                    };
                    TotalCost += serviceList.Cost;
                    serviceLists.Add(serviceList);

                }
            }
            if (!SavePersonPeriod)
            {
                var personPeriod = new PersonPeriod
                {
                    StartDate = DateStart,
                    EndDate = DateEnd,
                    TotalCost = TotalCost
                };
                var lastDay = DateTime.DaysInMonth(DateEnd.Year, DateEnd.Month);
                PersonPeriodData = personPeriod;
                db.PersonPeriods.Add(PersonPeriodData);
                db.SaveChanges();
                
                SavePersonPeriod = true;
            }
            var period = db.PersonPeriods.First(s => s.Id == PersonPeriodData.Id);
            period.TotalCost = TotalCost;
            db.SaveChanges();

            foreach (var serviceList in serviceLists)
            {
                serviceList.IdPersonPeriod = PersonPeriodData.Id;
            }
            ServicesListW = new ObservableCollection<ServiceListCosts>(serviceLists.Select(s => new ServiceListCosts
            {
                NameService = s.Service.Name,
                Cost = s.Cost
            }));
            var ServiceListTotalCost = new ServiceListCosts()
            {
                NameService = "Итого",
                Cost = TotalCost
            };
            ServicesListW.Add(ServiceListTotalCost);
            RequestHideMainWindow?.Invoke();
            RequestShowServiceListWindow?.Invoke();
        }

        public void Save()
        {
            using var db = new AppDbContext();
            var serviceListsSave = new List<ServicesList>();
            foreach (var sl in serviceLists)
            {
                var serviceList = new ServicesList
                {
                    IdService = sl.IdService,
                    Cost = sl.Cost,
                    LastMeterReading = sl.LastMeterReading,
                    IdPersonPeriod = sl.IdPersonPeriod
                };
                serviceListsSave.Add(serviceList);
            }
            db.ServicesLists.AddRange(serviceListsSave);
            db.SaveChanges();
            DateStartPicker = DateEndPicker;
            MessageBox.Show("Данные сохранены ! ! !");
            InputUpdate();
            SavePersonPeriod = false;
            if (HasMeteringDevice)
            {
                RequestShowMainWindow?.Invoke();
                RequestHideServiceListWindow?.Invoke();
            }
            else
            {
                RequestShowPersonPeriodWindow?.Invoke();
                RequestHideServiceListWindow?.Invoke();
            }

        }
        public void ButtonBackServiceListMindow()
        {
            if(HasMeteringDevice)
            {
                RequestShowMainWindow?.Invoke();
                RequestHideServiceListWindow?.Invoke();
            }
            else
            {
                RequestShowPersonPeriodWindow?.Invoke();
                RequestHideServiceListWindow?.Invoke();
            }

        }

        public void InputUpdate()
        {
            using var db = new AppDbContext();
            if (HasMeteringDevice)
            {
                Input = new ObservableCollection<UserInputData>(
               db.Services.Select(s => new UserInputData
               {
                   IdService = s.Id,
                   NameService = s.Name

               }).Where(s => s.IdService != 2).ToList());
            }
            else
            {
                Input = new ObservableCollection<UserInputData>(
               db.Services.Select(s => new UserInputData
               {
                   IdService = s.Id,
                   NameService = s.Name

               }).Where(s => s.IdService != 3 && s.IdService !=4).ToList());
            }
            
        }
        public void ButtonPersonPeriod()
        {
            
            HasMeteringDevice = false;
            InputUpdate();
            RequestShowPersonPeriodWindow?.Invoke();
            RequestHideMainWindow?.Invoke();
        }
        public void ButtonMainWindowShow()
        {
            HasMeteringDevice = true;
            InputUpdate();
            RequestShowMainWindow?.Invoke();
            RequestHidePersonPeriodWindow?.Invoke();
        }
        public void ButServHystoryCost()
        {
            using var db = new AppDbContext();
            PersonPeriods = new ObservableCollection<PersonPeriod>
           (
               db.PersonPeriods.Select(p => new PersonPeriod
               {
                   Id = p.Id,
                   StartDate = p.StartDate,
                   EndDate = p.EndDate,
                   TotalCost = p.TotalCost,
                   ServicesLists = p.ServicesLists.Select(s=> new ServicesList
                   {
                       Id = s.Id,
                       Cost = s.Cost,
                       IdPersonPeriod = s.IdPersonPeriod,
                       Service = s.Service,
                       PersonPeriod = s.PersonPeriod
                   }).ToList()
               }).ToList());
            RequestShowHistoryCostWindow?.Invoke();
            RequestHideMainWindow?.Invoke();

        }
        public void HistoryBack()
        {
            if (HasMeteringDevice)
            {
                RequestShowMainWindow?.Invoke();
                RequestHideHistoryCostWindow?.Invoke();
            }
            else
            {
                RequestShowPersonPeriodWindow?.Invoke();
                RequestHideHistoryCostWindow?.Invoke();
            }
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
            ButtonHistoryBack = new RelayCommand(HistoryBack);
            ButtonSave = new RelayCommand(Save);
            ButtonBack = new RelayCommand(ButtonBackServiceListMindow);
            ButtonEnterClick = new RelayCommand(EnterButton);
            ButtonHistoryClick = new RelayCommand(ButServHystoryCost);
            ButtonMainWindow = new RelayCommand(ButtonMainWindowShow);
            DbInitializer.Seed();
            Months = new ObservableCollection<Month>(GetDate());
            InputUpdate();
           

            ButtonNotMeteringDeviceClick = new RelayCommand(ButtonPersonPeriod);
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
                       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));



    }
}
