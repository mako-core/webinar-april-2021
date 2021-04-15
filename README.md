# Webinar-April-2021
This repo contains the code for the April 2021 Mako Webinar: Modernize your print workflow.

## Building
This repo does not contain a copy of the Mako SDK. In order to build the solution, you will need to drop in your copy of the Mako SDK SWIG NuGet package (e.g. MakoSDK-vc16-SWIG.6.0.1.290.nupkg) into:

```
Source\LocalPackages
```
After this, add the NuGet package to all project in the solution, using the NuGet package manager in Visual Studio, and selecting LocalPackages as the source.