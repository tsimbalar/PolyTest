PolyTest
========

Unit-testing tool for tests based on [one bad attribute](http://xunitpatterns.com/Derived%20Value.html#One Bad Attribute "One Bad Attribute pattern - xUnit Test Patterns") pattern.


When testing things such as validation of inputs, it is rather common to have to test different combinations of invalid inputs against our System Under Test (SUT). 

For example, imagine that you have a method `Validate()` taking into account an argument of type `Input`. It should return a "valid" result if `Input.Name` is not null and non-empty and also `Input.Age` is greater than 0. Testing this logic would result in test code that looks like this :

```csharp
[TestMethod]
public void Validate_with_input_with_null_Name_must_be_invalid()
{
    // Arrange
    var invalidInput = new Input { Name = null };
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(invalidInput);

    // Assert
    AssertIsInvalid(validationResult, "input with null Name should be invalid");
}



[TestMethod]
public void Validate_with_input_with_empty_Name_must_be_invalid()
{
    // Arrange
    var invalidInput = new Input { Name = String.Empty };
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(invalidInput);

    // Assert
    AssertIsInvalid(validationResult, "input with empty Name should be invalid");
}

[TestMethod]
public void Validate_with_input_with_Age_minus_one_must_be_invalid()
{
    // Arrange
    var invalidInput = new Input { Name = "someName", Age = -1 };
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(invalidInput);

    // Assert
    AssertIsInvalid(validationResult, "input with Age -1 should be invalid");
}
```

The problem with that approach is that, as the validation conditions get trickier and combine conditions on several properties of the input, your test code gets messier and messier: whenever a new condition is introduced on property `Input.XXX`, you have to fix all the other tests so that the initial condition sets up Input with a valid value of `XXX`. 

For ease of maintenance and readability, you can apply one of the patterns listed in the Unit-testing pattern catalog [xUnit Test Patterns](http://xunitpatterns.com/) : **[One Bad Attribute](http://xunitpatterns.com/Derived%20Value.html#One Bad Attribute)**.

The general idea is, for each of the conditions to test, **start from a valid case** and **change the input just enough to invalidate the condition**. To ease code maintenance, the creation of the valid input should be done in a Test Creation method, i.e. a small helper method that centralizes the creation of that valid input, so that changes in the definition of what is considered valid only involve one change in the test code.

In our example, this would mean doing something like that : 

```csharp
[TestMethod]
public void Validate_with_valid_input_must_be_valid()
{
    // Arrange
    var input = MakeValidInput();
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(input);

    // Assert
    AssertIsValid(validationResult, "valid case should be valid (duh!)");
}

[TestMethod]
public void Validate_with_input_with_null_Name_must_be_invalid()
{
    // Arrange
    var input = MakeValidInput();
    input.Name = null;
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(input);

    // Assert
    AssertIsInvalid(validationResult, "input with null Name should be invalid");
}

[TestMethod]
public void Validate_with_input_with_empty_Name_must_be_invalid()
{
    // Arrange
    var input = MakeValidInput();
    input.Name = String.Empty;
    var sut = MakeSUT();

    // Act
    var validationResult = sut.Validate(input);

    // Assert
    AssertIsInvalid(validationResult, "input with empty Name should be invalid");
}

[TestMethod]
public void Validate_with_input_with_Age_minus_one_must_be_invalid()
{
    // Arrange
    var input = MakeValidInput();
    input.Age = -1;
    var sut = new Validator();

    // Act
    var validationResult = sut.Validate(input);

    // Assert
    AssertIsInvalid(validationResult, "input with Age -1 should be invalid");
}
```

where `MakeValidInput()` could look like this :
```csharp
private static Input MakeValidInput()
{
    return new Input
    {
        Age = 4,
        Name = "a valid name"
    };
}
```
and would probably change depending on the specifications of the Validation.

Note that we also cover the validity of the valid case, as it allows to prove that, in some conditions, the validation does indeed succeed.



TO CONTINUE : 
- include example applying that pattern
- see that is a lot of tests and hard to maintain
- see how it can be improved (losing some benefits though..)
