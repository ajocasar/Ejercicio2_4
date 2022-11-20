using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio2_4.Model
{
    public class Video
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string name { get; set; }
        public byte[] video { get; set; }
        public string path { get; set; }
        public string date { get; set; }
    }
}
