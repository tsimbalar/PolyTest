using System.Collections.Generic;

namespace PolyTest.Examples.CarExample.Models
{
    public class Car
    {
        public Car()
        {
            Wheels = new List<Wheel>();
            NeedsEngine = true;
        }

        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public int NumberOfDoors { get; set; }
        public bool NeedsEngine { get; set; }
        public Engine Engine { get; set; }
        public IList<Wheel> Wheels { get; set; }
    }
}