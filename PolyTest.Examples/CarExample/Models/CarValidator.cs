using System.Linq;

namespace PolyTest.Examples.CarExample.Models
{
    public class CarValidator
    {
        public bool Validate(Car car)
        {
            var isValid = true;
            if (car.NumberOfDoors <= 0) isValid = false;
            if (car.BrandName == null) return false;
            if (string.IsNullOrWhiteSpace(car.ModelName)) isValid = false;

            if (car.NeedsEngine)
            {
                if (car.Engine == null) isValid = false;
                else
                {
                    // validate engine
                    if (!ValidateEngine(car.Engine)) isValid = false;
                }

            }
            else
            {
                if (car.Engine != null) isValid = false; // if NeedsEngine is false, Engine must be null
            }
            if (!car.Wheels.Any())
            {
                isValid = false; // must have some wheels
            }
            if (car.Wheels.Count > 4)
            {
                isValid = false; // must have no more than 4 wheels
            }

            return isValid;
        }

        private bool ValidateEngine(Engine engine)
        {
            return true;
        }
    }
}