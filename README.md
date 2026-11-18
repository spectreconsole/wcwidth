# Wcwidth

_[![Wcwidth NuGet Version](https://img.shields.io/nuget/v/Wcwidth.svg?style=flat&label=NuGet%3A%20Wcwidth)](https://www.nuget.org/packages/Wcwidth)_

This is a port of the [Python port](https://github.com/jquast/wcwidth) 
written by Jeff Quast, which originally was written by Markus Kuhn.

* Python port: https://github.com/jquast/wcwidth (MIT)
* Original: http://www.cl.cam.ac.uk/~mgk25/ucs/wcwidth.c

## Usage

```csharp
using Wcwidth;

// Get the width
var width = UnicodeCalculator.GetWidth('コ');

// It should be 2 cells wide
Debug.Assert(width == 2);

// Strings and spans are measured per Unicode scalar,
// including emoji sequences, and return -1 if they
// contain a control character
Debug.Assert(UnicodeCalculator.GetWidth("コンニチハ") == 10);
Debug.Assert(UnicodeCalculator.GetWidth("👨‍👩‍👧".AsSpan()) == 2);
```

### Source package

The `Wcwidth.Sources` package compiles the source code into your project.
These compilation symbols change what it compiles:

* `WCWIDTH_VISIBILITY_INTERNAL` makes the types `internal` instead of `public`.
* `WCWIDTH_SYSTEM_MEMORY` adds `GetWidth(ReadOnlySpan<char>)` on targets
  without a built-in `ReadOnlySpan<T>`, such as .NET Standard 2.0. Define it
  when your project references the `System.Memory` package.

## Building

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet make
```

## Acknowledgement

This code is a port of https://github.com/jquast/wcwidth,
licensed under [MIT](https://github.com/jquast/wcwidth/blob/dc720a9a4c3c6ae6c5b16a552cfe5186dde22551/LICENSE).

This code was originally derived directly from C code of the same name, 
whose latest version is available at http://www.cl.cam.ac.uk/~mgk25/ucs/wcwidth.c:

```
* Markus Kuhn -- 2007-05-26 (Unicode 5.0)
* Permission to use, copy, modify, and distribute this software
* for any purpose and without fee is hereby granted. The author
* disclaims all warranties with regard to this software.
```