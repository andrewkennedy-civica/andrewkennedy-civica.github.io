using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterJsonCSV.GeoJson
{
    public class Geometry
    {
        public string type { get; set; }        
        public List<List<List<double>>> coordinates { get; set; }
    }
}
