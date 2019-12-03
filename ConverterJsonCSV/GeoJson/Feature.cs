using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterJsonCSV.GeoJson
{
    public class Feature
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }
}
