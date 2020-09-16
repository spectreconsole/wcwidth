# Wcwidth

This is a port of the [Python port](https://github.com/jquast/wcwidth) 
written by Jeff Quast, which originally was written by Markus Kuhn.

* Python port: https://github.com/jquast/wcwidth (MIT)
* Original: http://www.cl.cam.ac.uk/~mgk25/ucs/wcwidth.c

## Usage

```csharp
using Wcwidth;

// Get the width
var width = Wcwidth.GetWidth('コ');

// It should be 2 cells wide
Debug.Assert(width == 2);
```

## Building

We're using [Cake](https://github.com/cake-build/cake) as a 
[dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) 
for building. So make sure that you've restored Cake by running 
the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
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