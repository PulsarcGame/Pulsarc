# Code Style

The following is a general guide of how to structure your code. When contributing to Pulsarc it is preferrable that your code follows these guidelines.

**NOTE: Not all of Pulsarc's codebase may follow these guidelines exactly. We hope to make the code style as uniform as possible as time goes on.**

This guide is heavily inspired by [Quaver's Code Style Guide](https://github.com/Quaver/Quaver/blob/develop/CODESTYLE.md) and [MonoGame's Code Style Guide](https://github.com/MonoGame/MonoGame/blob/develop/CODESTYLE.md).

### Useful Links ###
* [C# Coding Conventions (MSDN)](http://msdn.microsoft.com/en-us/library/ff926074.aspx)

# Guidelines

## Tabs and Indenting ##
**DO NOT** use tab characters (\0x09)

**DO** use 4 space characters for each indentation.

### Braces
**DO** place open and closed braces on a new line.

**DO NOT** ignore braces on single-statement blocks.

**DO NOT** use one-liners, exception for Guard Clauses.

**DO NOT** put braces on the same line as a expression, exception for Guard Clauses.

### Guard Clauses
**DO** use guard clauses over nested condtionals. More info [Here](https://www.refactoring.com/catalog/replaceNestedConditionalWithGuardClauses.html)

**Example**
```cs
if (!condition) { return; }

...

if (!secondCondtion) { return; }
```

### Switches
**DO** indent ``case`` statements from the ``switch`` statement.

**Example:**
```cs
switch (someExpression)
{
    case 0:
        DoSomething();
        break;

    case 1:
        DoSomethingElse();
        break;
}
```

### Single line Property Statements
Single line property statements can have braces that begin and end on the same line. Add a single space before and after the braces.

**Example:**
```cs
internal class Foo
{
   internal int Bar { get; set; } = 10;
}
```

### Multi-Line Property Statements
Multi-line property statements must have braces on new lines.

**Example:**
```cs
internal class Foo
{
   internal int Bar
   {
      get => Bar * 2
      set { bar = value; }
   }
}
```

## Commenting
Comments should be used to describe intention, algorithmic overview, and/or logical flow.  It would be ideal, if from reading the comments alone, someone other than the author could understand a functions intended behavior and general operation. While there are no minimum comment requirements (and certainly some very small routines need no commenting at all), it is hoped that most routines will have comments reflecting the programmers intent and approach.

Comments must provide added value or explanation to the code. Simply describing the code is not helpful or useful.

**Example:**
```cs
    // Wrong
    // Set count to 1
    count = 1;

    // Right
    // Set the initial reference count so it isn't cleaned up next frame
    count = 1;
```

### Documentation Comments
**DO** use XML doc comments on all methods.

**DO** Fill out at least the ``<summary>`` of the XML doc comment.

**Example:**
```cs
public class Foo 
{
    /// <summary>This is my awesome method B)</summary>
    /// <param name="bar">What a neat parameter!</param>
    /// <returns>Some really cool stuff!</returns>
    public int MyMethod(int bar)
    {
        ...
    }
}
```

### Comment Style
**DO** use ``//`` (two slashes) style of comment tags in most situations.

**DO** place comments above the code instead of besides it.

**Example:**
```cs
    // This is required for WebClient to work through the proxy
    GlobalProxySelection.Select = new WebProxy("http://itgproxy");

    // Create object to access Internet resources
    WebClient myClient = new WebClient();
```

## Spacing
Spaces improve readability by decreasing code density. Here are some guidelines for the use of space characters within code:

* **DO** use a single space after a comma between function arguments.
```cs
Console.In.Read(myChar, 0, 1);  // Right
Console.In.Read(myChar,0,1);    // Wrong
```
* **DO NOT** use a space after the parenthesis and function arguments
```cs
CreateFoo(myChar, 0, 1)         // Right
CreateFoo( myChar, 0, 1 )       // Wrong
```
* **DO NOT** use spaces between a function name and parenthesis.
```cs
CreateFoo()                     // Right
CreateFoo ()                    // Wrong
```
* **DO NOT** use spaces inside brackets.
```cs
x = dataArray[index];           // Right
x = dataArray[ index ];         // Wrong
```
* **DO** use a single space before flow control statements
```cs
while (x == y)                  // Right
while(x==y)                     // Wrong
```
* **DO** use a single space before and after binary operators
```cs
if (x == y)                     // Right
if (x==y)                       // Wrong
```
* **DO NOT** use a space between a unary operator and the operand
```cs
++i;                            // Right
++ i;                           // Wrong
```
* **DO NOT** use a space before a semi-colon. Do use a space after a semi-colon if there is more on the same line
```cs
for (int i = 0; i < 100; ++i)   // Right
for (int i=0 ; i<100 ; ++i)     // Wrong
```

## `var` vs Type
**DO** use the variable type in most cases.

**DO NOT** use `var`, unless the type of the var is explicitly labeled on the right side during assignment.

## Naming
camelCasing - First word all lowercase, following words initial uppercase.

PascalCasing - All words initial uppercase.

ALL\_CAPS - All letters are uppercase, underscore (_) between words

* **DO NOT** use the Hungarian notation (including the type of the variable in the name)
* **DO** use camelCasing for private member variables.
* **DO** use camelCasing for parameters.
* **DO** use camelCasing for local variables
* **DO** use PascalCasing for method, property, event, and class names.
* **DO** use PascalCasing for protected member variables.
* **DO** Use ALL_CAPS for static variables.
* **DO** prefix interface names with ``I``
* **DO NOT** prefix enums, classes, or delegates with any letter.

Follow the [.NET Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) for anything not covered.
