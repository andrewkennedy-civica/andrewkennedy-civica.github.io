using ChoETL;
using ConverterJsonCSV.GeoJson;
using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ConverterJsonCSV
{
    class Program
    {
        static Random random = new Random();

        static Dictionary<string, double> WeaversCourtCoords = new Dictionary<string, double>()
        {
            { "A_min_X", -5.9407937 },
            { "A_max_X", -5.9402634 },

            { "A_min_Y", 54.5914832 },
            { "A_max_Y", 54.5919338 },

            { "B_min_X", -5.9412135 },
            { "B_max_X", -5.9406990 },

            { "B_min_Y", 54.5915435 },
            { "B_max_Y", 54.5917769 }
        };

        static void Main(string[] args)
        {
            RunJsonGenerator();
        }

        public static string JsonToCSV(string jsonContent)
        {
            StringBuilder sb = new StringBuilder();
            using (var p = ChoJSONReader.LoadText(jsonContent))
            {
                using (var w = new ChoCSVWriter(sb).WithFirstLineHeader())
                {
                    w.Write(p);
                }                   
            }
            return sb.ToString();
        }

        public static void RunJsonConverter()
        {
            Console.WriteLine("Enter full path to JSON file including file and extension");
            var jsonFilePath = Console.ReadLine();
            Console.WriteLine("Enter full output path");
            var csvPath = Console.ReadLine();
            Console.WriteLine("Working...");
            string json = File.ReadAllText(jsonFilePath);
            string csv = JsonToCSV(json);
            File.WriteAllText(csvPath, csv);
            Console.WriteLine("Finished...");
        }

        public static string GenerateDummyGeoJson(int numPoints)
        {
            //create a geojson geometry object with a random x & y

            WeaversCourtCoords.TryGetValue("A_min_X", out double A_min_X);
            WeaversCourtCoords.TryGetValue("A_max_X", out double A_max_X);
            WeaversCourtCoords.TryGetValue("A_min_Y", out double A_min_Y);
            WeaversCourtCoords.TryGetValue("A_max_Y", out double A_max_Y);

            WeaversCourtCoords.TryGetValue("B_min_X", out double B_min_X);
            WeaversCourtCoords.TryGetValue("B_max_X", out double B_max_X);
            WeaversCourtCoords.TryGetValue("B_min_Y", out double B_min_Y);
            WeaversCourtCoords.TryGetValue("B_max_Y", out double B_max_Y);

            FeatureCollection rootObject = new FeatureCollection()
            {
                type = "FeatureCollection",
                features = new List<Feature>()
            };

            for (int i=0; i < numPoints; i++)
            {
                Feature featureA = new Feature()
                {
                    type = "Feature",
                    properties = new Properties()
                    {
                        description = "blank",
                        count = GetRandomInt(1, 11),
                    },
                    geometry = new Geometry()
                    {
                        type = "Polygon",
                        coordinates = GetRandomCoordinate(A_min_X, A_max_X, A_min_Y, A_max_Y)
                    }
                };
                
                featureA.properties.description = featureA.geometry.coordinates[0][0][0].ToString() + ", " + featureA.geometry.coordinates[0][1][1].ToString();

                Feature featureB = new Feature()
                {
                    type = "Feature",
                    properties = new Properties()
                    {
                        description = "blank",
                        count = GetRandomInt(1, 11)
                    },
                    geometry = new Geometry()
                    {
                        type = "Polygon",
                        coordinates = GetRandomCoordinate(B_min_X, B_max_X, B_min_Y, B_max_Y)
                    }
                };

                featureB.properties.description = featureB.geometry.coordinates[0][0][0].ToString() + ", " + featureB.geometry.coordinates[0][1][1].ToString();

                rootObject.features.Add(featureA);
                rootObject.features.Add(featureB);
            }

            StringBuilder GeoJson = new StringBuilder();

            var SingleJson = JsonConvert.SerializeObject(rootObject);
            GeoJson.Append(SingleJson);

            GeoJson = GeoJson.Replace(@"\", "");
           
            return GeoJson.ToString();
        }

        public static List<List<List<double>>> GetRandomCoordinate(double minimumX, double maximumX, double minimumY, double maximumY)
        {
            var X = random.NextDouble() * (maximumX - minimumX) + minimumX;
            var Y = random.NextDouble() * (maximumY - minimumY) + minimumY;

            double offset = 0.000006;
            //top left
            List<double> polygonStartPt = new List<double>()
            {
                X,
                Y
            };
         
            var offsetXTopR = X + (offset*2);
            //top right of square, adjacent to starting point (top left)
            List<double> polygonSecondPt = new List<double>()
            {
                offsetXTopR,
                Y
            };

            var offsetYBtmR = Y + offset;
            //bottom right, below 2nd pt on same X
            List<double> polygonThirdPt = new List<double>()
            {
                offsetXTopR,
                offsetYBtmR
            };

            //4th point, on same X as 1st point and same Y as 3rd point
            List<double> polygonFourthPt = new List<double>()
            {
                X,
                offsetYBtmR
            };

            List<double> polygonEndPt = new List<double>()
            {
                X,
                Y
            };

            List<List<List<double>>> polygonPointList = new List<List<List<double>>>()
            {
                new List<List<double>>()
                {
                polygonStartPt,
                polygonSecondPt,
                polygonThirdPt,
                polygonFourthPt,
                polygonEndPt
                }             
            };

            return polygonPointList;
        }

        public static int GetRandomInt(int min, int max)
        {
            Random rnd = new Random();

            return rnd.Next(min, max);
        }

        public static void RunJsonGenerator()
        {
            StringBuilder GeoJson = new StringBuilder();
            string Points = "";

            Console.WriteLine("How many data points to create? Even numbers please...");
            int.TryParse(Console.ReadLine(), out int numDataPoints);

            //int numDataPoints = 60;

            Console.WriteLine("Enter full output path");
            var writePath = Console.ReadLine();

            //var writePath = "C:/Users/andrew.kennedy/Documents/MapsSensAI/Mapbox/geo60.json";

            if (numDataPoints % 2 == 0)
            {
                numDataPoints = numDataPoints / 2;
            }
            else
            {
                numDataPoints = +1;
                numDataPoints = numDataPoints / 2;
            }

            Points = GenerateDummyGeoJson(numDataPoints);
            GeoJson.Append(Points);

            var GeoJsonArray = JsonConvert.SerializeObject(
                new[]
                {
                     GeoJson.ToString()
                });
            
            GeoJsonArray = GeoJsonArray.Replace(@"\", "");
            GeoJsonArray = GeoJsonArray.Replace("[\"", "");
            GeoJsonArray = GeoJsonArray.Replace("]}}]}\"]", "]}}]}");
            File.WriteAllText(writePath, GeoJsonArray);
            Console.WriteLine("Finished");
        }
    }
}