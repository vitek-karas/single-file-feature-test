# Tests for functional behavior of .NET 5 single-file

To run just execute the `FeatureTest` console application. It does not use any test runner, just a very simple reflection based loop.

To run as self-contained single file, go to `FeatureTest` directory and run
```console
dotnet publish -r win-x64 --self-contained true /p:PublishSingleFile=true -o bin\sf-scd
bin\sf-scd\FeatureTest.exe
```