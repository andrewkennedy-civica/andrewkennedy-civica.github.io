using System;
using System.Collections.Generic;
using System.Text;

namespace ConverterJsonCSV.GeoJson
{
    public class FeatureCollection
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }
}
