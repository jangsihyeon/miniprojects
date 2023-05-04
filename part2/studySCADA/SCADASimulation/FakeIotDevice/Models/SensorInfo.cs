using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeIotDevice.Models
{
    public class SensorInfo
    {
        public String Home_Id { get; set; }
        public String Room_Name { get; set; }
        public DateTime Sensing_DateTime { get; set; }
        public float Temp { get; set; }
        public float Humid { get; set; }
    }
}
