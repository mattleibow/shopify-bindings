#addin "Cake.Xamarin"
#addin "Cake.XCode"

#load "common.cake"

var IOS_VERSION = "1.2.5";
var IOS_URL = string.Format ("https://github.com/Shopify/mobile-buy-sdk-ios/archive/{0}.zip", IOS_VERSION);

CakeSpec.Libs = new ISolutionBuilder [] {
	new DefaultSolutionBuilder {
		SolutionPath = "./source/Shopify.sln",
		Configuration = "Release",
		OutputFiles = new [] { 
			new OutputFileCopy {
				FromFile = "./source/Shopify.iOS/bin/Release/Shopify.iOS.dll",
				ToDirectory = "./output/ios"
			},
		}
	},	
};

CakeSpec.Samples = new ISolutionBuilder [] {
	new IOSSolutionBuilder { SolutionPath = "./samples/ShopifyiOSSample.sln" },
};

CakeSpec.NuSpecs = new [] {
    "nuget/Xamarin.Shopify.iOS.nuspec",
};

Task ("externals-ios")
    .IsDependentOn ("externals-base")
    .Does (() => 
{
	if (!DirectoryExists ("./externals/ios"))
		CreateDirectory ("./externals/ios");

    if (!FileExists ("./externals/mobile-buy-sdk-ios.zip"))
        DownloadFile (IOS_URL, "./externals/mobile-buy-sdk-ios.zip");
        
    var temp = "./externals/mobile-buy-sdk-ios-" + IOS_VERSION;
	if (!DirectoryExists (temp))
	   Unzip ("./externals/mobile-buy-sdk-ios.zip", "./externals/");

   if (!FileExists ("./externals/ios/libMobile-Buy-SDK.a")) {
        BuildXCode (
            project: "Mobile Buy SDK.xcodeproj", 
            target: "Buy Static", 
            libraryTitle: "Mobile-Buy-SDK", 
            output: "Buy.framework/Buy", 
            workingDirectory: temp + "/Mobile Buy SDK/");
        CopyFile (temp + "/Mobile Buy SDK/libMobile-Buy-SDK.a", "./externals/ios/libMobile-Buy-SDK.a");
   }
});

Task ("externals")
    .IsDependentOn ("externals-ios")
    .IsDependentOn ("externals-base")
    .Does (() => 
{
});

Task ("clean")
    .IsDependentOn ("clean-base")
    .Does (() =>
{
	DeleteFiles ("./externals/ios/Podfile.lock");
	CleanXCodeBuild ("./Pods/");
});

DefineDefaultTasks ();

RunTarget (TARGET);
