namespace PolyTest.Examples.InputValidationExample.Models
{
    public class Input
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public bool HasCheezburger { get; set; }

        public Cheezburger Cheezburger { get; set; }
    }

    public class Cheezburger
    {

    }
}