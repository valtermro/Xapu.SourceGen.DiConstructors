# SourceGen.DiConstructor

A C# source generator that handles the constructor boilerplate for dependency injection.

At its current state, this project is more of an experiment than a useful software.


## How it works

It searches for all **partial** classes with **fields** annotated with the
`Injected` attribute and generates a constructor that initializes those fields.

**Example**

Given the following class:

```csharp
using Xapu.SourceGen.DiConstructors;

public partial class Example
{
    [Injected] private readonly Stub _dependency;
}
```

The following code is generated:

```csharp
public partial class Example
{
    public Example(Stub _dependency)
    {
        this._dependency = _dependency;
    }
}
```


## With inheritance

If a class inherits from another with `Injected` fields, the child's
constructor will collect the parent's dependencies as well.

**Example**

Given the following classes:

```csharp
using Xapu.SourceGen.DiConstructors;

partial class Parent
{
    [Injected] private readonly Stub _parentDependency;
}

partial class Child : Parent
{
    [Injected] private readonly Stub _childDependency;
}
```

The following is generated:

```csharp
partial class Parent
{
    public Parent(Stub _parentDependency)
    {
        this._parentDependency = _parentDependency;
    }
}

partial class Child
{
    public Child(Stub _parentDependency, Stub _childDependency)
        : base(_parentDependency)
    {
        this._childDependency = _childDependency;
    }
}
```


## The post-constructor hook

Because the constructor is generated, we cannot use it to do anything during object initialization.  
To workaround this, if a  method named `AfterConstructor` is declared in the class,
it'll be called **at the end** of the generated constructor.

**Example**

Given the following classes:

```csharp
partial class Parent
{
    // [Injected] ...

    private void AfterConstructor()
    {
        // do parent work
    }
}

partial class Child : Parent
{
    // [Injected] ...

    private void AfterConstructor()
    {
        // do child work
    }
}
```

The following is generated:

```csharp
partial class Parent
{
    public Parent(/* ... dependencies */)
    {
        // parent field initialization...
        AfterConstructor();
    }
}

partial class Child
{
    public Child(/* dependencies */) : base(/* parent dependencies */)
    {
        // child field initialization...
        AfterConstructor();
    }
}
```

### Notes on the "post-constructor"

1. It must not have required parameters, the constructor will call it without arguments.
1. Cannot be overridden. Overrides would be called in place of the base's method and that would be really bad.
1. Aside from the "no required parameters" rule, any signature is valid.


## Installing

There is a nuget package avaliable that contains both the generator and the `Injected` attribute.

```xml
<ItemGroup>
    <PackageReference Include="Xapu.SourceGen.DiConstructors" Version="x.x.x" />
</ItemGroup>
```

*If you have some kind of "core" project that is referenced by all projects in your solution,
you could install this package to that project and the other projects would have it.*