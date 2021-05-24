using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationPrism.Models
{
    public class Position
    {
        public Position() { }

        public Position(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        [PrimaryKey] [AutoIncrement]
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Time { get; set; }
    }
}
