# Shopify Bindings

This repo is a set of Xamarin bindings for Shopify's 
Mobile Buy SDK.

Although it works, it is still a bit of work in progress.
Currently only iOS is bound. Android comming soon.

## Building

This repo makes use of [Cake Build](http://cakebuild.net).

To build the managed libraries:

    ./bootstrapper.sh -t libs

This will produce the `Shopify.iOS.dll` in the `output` folder.

To build the samples, you need to make sure that the libraries are 
built first before using the IDE. However, this can be done via command 
line:

    ./bootstrapper.sh -t samples

To produce a "final" packaged product:

    ./bootstrapper.sh -t component

This will produce a package in the `output` folder.
Currently it is just a NuGet, but a Xamarin component is coming soon.
