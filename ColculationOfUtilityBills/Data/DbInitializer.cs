using ColculationOfUtilityBills.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColculationOfUtilityBills.Data
{
    public class DbInitializer
    {
        public static void Seed()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            if (db.Services.Any()) return;
            string filePath = "Res/dataServices.txt";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден: " + filePath);
                return;
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var serv = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (serv.Length >= 4)
                {
                    try
                    {
                        var service = new Service()
                        {
                            Name = string.Join(" ", serv.Take(serv.Length - 3)),
                            Rate = decimal.Parse(serv[^3].Replace(',', '.'), CultureInfo.InvariantCulture),
                            Standart = decimal.Parse(serv[^2].Replace(',', '.'), CultureInfo.InvariantCulture),
                            MeasureUnit = serv[^1]
                        };
                        db.Services.Add(service);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                    Console.WriteLine("Недостаточно данных в строке");
            }
            db.SaveChanges();
        }
    }
}
