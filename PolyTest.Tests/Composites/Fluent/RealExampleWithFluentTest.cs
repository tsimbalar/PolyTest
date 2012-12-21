using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tree.Fluent;
using PolyTest.Tree.Fluent.Magic;

namespace PolyTest.Tests.Composites.Fluent
{
    [TestClass]
    public class RealExampleWithFluentTest
    {

        [TestMethod]
        public void ValidCase()
        {

            var sut = new CarValidator();
            Assert.AreEqual(true, sut.Validate(MakeValidCar()), "valid case should be valid (duh)");

        }

        [TestMethod]
        public void ValidationExampleTest()
        {
            var sut = new CarValidator();
            TestTree.From("Valid Car", MakeValidCar)
                    .With(c => c.NumberOfDoors, 0)
                    .With(c => c.NumberOfDoors, -1)
                    .WithNull(c => c.BrandName)
                    .WithNullOrWhitespace(c => c.ModelName )
                    .With(c => c.NeedsEngine, false,
                        fromThere=> fromThere.IgnoreSelf()
                            .With(c=> c.Engine, new Engine())
                    )
                    .With(c => c.NeedsEngine, true,
                        fromHere => fromHere.IgnoreSelf()
                            .WithNull(c=> c.Engine)
                    )
                    .Walk(
                        act: o => sut.Validate(o),
                        assert: AssertIsInvalid)
                    .AssertIsNotFailed();

        }

        private static void AssertIsInvalid(bool validationResult)
        {
            Assert.AreEqual(false, validationResult, "Should be false to be considered invalid");
        }

        private Car MakeValidCar()
        {
            var c = new Car();
            c.BrandName = "Renault";
            c.ModelName = "Megane";
            c.NumberOfDoors = 3;
            // add 4 wheels
            Enumerable.Range(0, 4).ToList()
                .ForEach(i => c.Wheels.Add(new Wheel(c, i)));
            c.Engine = new DieselEngine();
            return c;
        }
    }



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
            return isValid;
        }

        private bool ValidateEngine(Engine engine)
        {
            return true;
        }
    }

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

    internal class DieselEngine : Engine
    {
    }

    public class Wheel
    {
        public Wheel(Car car, int index)
        {

        }
    }

    public class Engine
    {
    }
}
