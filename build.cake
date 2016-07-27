#addin "Cake.Xamarin"
#addin "Cake.XCode"

#load "common.cake"

var IOS_VERSION = "1.2.6";
var IOS_URL = string.Format ("https://github.com/Shopify/mobile-buy-sdk-ios/archive/{0}.zip", IOS_VERSION);

var ANDROID_VERSION = "1.2.4";
var ANDROID_URL = string.Format ("https://bintray.com/shopify/shopify-android/download_file?file_path=com%2Fshopify%2Fmobilebuysdk%2Fbuy%2F{0}%2Fbuy-{0}.aar", ANDROID_VERSION);

CakeSpec.Libs = new ISolutionBuilder [] {
	new DefaultSolutionBuilder {
		SolutionPath = "./source/Shopify.sln",
		Configuration = "Release",
		OutputFiles = new [] { 
			new OutputFileCopy {
				FromFile = "./source/Shopify.iOS/bin/Release/Shopify.iOS.dll",
				ToDirectory = "./output/ios"
			},
			new OutputFileCopy {
				FromFile = "./source/Shopify.Android/bin/Release/Shopify.Android.dll",
				ToDirectory = "./output/android"
			},
		}
	},	
};

CakeSpec.Samples = new ISolutionBuilder [] {
	new IOSSolutionBuilder { SolutionPath = "./samples/ShopifyiOSSample.sln" },
	new IOSSolutionBuilder { SolutionPath = "./samples/ShopifyAndroidSample.sln" },
};

CakeSpec.NuSpecs = new [] {
    "nuget/Xamarin.Shopify.iOS.nuspec",
    "nuget/Xamarin.Shopify.Android.nuspec",
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

Task ("externals-android")
    .IsDependentOn ("externals-base")
    .Does (() => 
{
	if (!DirectoryExists ("./externals/android"))
		CreateDirectory ("./externals/android");

    if (!FileExists ("./externals/android/mobile-buy-sdk-android.aar"))
        DownloadFile (ANDROID_URL, "./externals/android/mobile-buy-sdk-android.aar");
});

Task ("externals")
    .IsDependentOn ("externals-android")
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
	CleanXCodeBuild ("./ios/");
});

DefineDefaultTasks ();

RunTarget (TARGET);
