﻿using ColculationOfUtilityBills.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColculationOfUtilityBills.Views
{
    /// <summary>
    /// Логика взаимодействия для PersonPeriodWindow.xaml
    /// </summary>
    public partial class PersonPeriodWindow : Window
    {
        public PersonPeriodWindow()
        {
            InitializeComponent();
            
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]*(?:[.,][0-9]*)?$");
        }
    }
}
