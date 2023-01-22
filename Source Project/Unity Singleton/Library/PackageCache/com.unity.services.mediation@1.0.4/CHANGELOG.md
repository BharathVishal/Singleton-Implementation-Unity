# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.4] - 2022-08-31

### Fixed
- Removed more verbose logging in batchmode of certain libraries interfering with tests

## [1.0.3] - 2022-08-30

### Fixed
- Removed verbose logging in batchmode of certain libraries interfering with tests

## [1.0.2] - 2022-08-30

### Fixed
- Fixed minor issue with Mobile Dependency Resolver

## [1.0.1] - 2022-08-30

### Fixed
- Fixed minor issue with Mobile Dependency Resolver

### Changed
- iOS and Android dependencies are now set as optimistic, not set to a minor version

## [1.0.0] - 2022-08-30

### Changed
- Update samples & code generator

## [0.5.1-preview.1] - 2022-07-28

### Added
- iOS: When building for iOS, an error log now alerts the developer that no iOS Advertisement Support package is present.

### Fixed
- Android: ShowOptions now accepts an empty S2SData structure.
- iOS: Fixed il2cpp crash issues.


## [0.5.0-preview.4] - 2022-06-02

### Added
- The Adapter Settings window now identifies adapters that are configured on the dashboard, and warns the user if the adapter is configured and not installed or the other way around.
- Banner support for Unity, AdColony, AdMob, AppLovin, and Vungle.
- Banner UI component to facilitate banner integration in the Unity UI.
- Support for Snapchat Ad Network, interstitial and rewarded Header Bidding ads. 
- Support for Mintegral Ad Network, interstitial and rewarded Header Bidding ads. 
- iOS: Added a build settings option to toggle support for dynamic linking on iOS.

### Changed
- Reduced the amount of logging in the Editor so only calls on unsupported platforms generate warnings.
- Mock ads now only show in the Editor when targeting a supported platform. For example, trying to load an ad while targeting standalone will log a warning.
- Added Async/Await API and deprecated the former API.

### Fixed
- Fixed a bug where the Ad Unit list search did not refresh the list in Unity 2022.
- iOS: Fixed potential issues that could occur when attempting to build for iOS with various EDM4U versions for dynamic linking.
- The no Game ID error log now only appears if the platform is supported.
- Fixed an issue where switching PSR versions caused a compile issue that would stop the Editor from importing a newer version of PSR without removing UMediation.

## [0.4.1-preview.1] - 2022-03-29

### Fixed
- Android: Fixed an issue where some devices did not support a default constructor for S2SRedeemData through JNI.

### Changed
- iOS: Version restriction for the adapters in anticipation of a dependency tree structure change.

## [0.4.0-preview.2] - 2022-02-16

### Added
- Added an optional ShowOptions parameter to UnsupportedRewardedAd.

## [0.4.0-preview.1] - 2022-02-01
### Added
- Added server-to-server redeem callback support.
    - RewardedAdShowOptions class to pass optional arguments when showing a rewarded ad.
    - S2SRedeemData struct to pass server-to-server redeem data.

### Changed
- ImpressionData.PublisherRevenuePerImpression type changed from string to double.
- ImpressionData.PublisherRevenuePerImpressionInMicros type changed from string to Int64.
- RewardedAd's Show function now accepts an optional RewardedAdShowOptions parameter.

### Fixed
- Editor mock rewarded ads no longer trigger the reward callback if they are skipped.


## [0.3.0-preview.3] - 2021-12-01

### Added
- MediationService.Instance.SdkVersion: Gets the native Mediation version at runtime. 
- Line numbers for generated code in the Code Generation window.
- PIPL Support for DataPrivacy Laws Enum
    - DataPrivacy.PIPLAdPersonalization - Personal Information Protection Law, regarding ad personalization, applicable to users residing in China.
    - DataPrivacy.PIPLDataTransport - Personal Information Protection Law, regarding moving data out of China, applicable to users residing in China

### Changed
- Refined Code Generation Code
    - Class Name includes Ad Type to avoid class name conflicts. (MyExampleAdClass -> InterstitialAdExample)
    - Now uses the override game ID initialization flow.
    - Added OnClicked and OnClosed Callbacks to code snippet.
    - Fixed Newline issues with OnUserRewarded Callback placement.
    - Renamed Event Args parameter in Ad Loaded (sargs -> args).
    - Renamed OnUserRewarded subscribed method (OnUserRewarded -> UserRewarded)
    - Changed code snippet color to increase visibility.
- Banner ads are now be excluded from the code generator because they are currently not supported.
    
### Fixed
- Fixed an issue where Unity Ads sometimes did not appear as installed in the Mediation Settings window.
- Fixed a misalignment of the adapter status and its icon on Windows for Unity 2020+.
- Fixed an issue where dark color palettes were used in some Mediation UI when using the Light Theme. 

## [0.2.1-preview.2] - 2021-10-12

### Added
- Test validating Gradle version to display a more meaningful error message.
- `InitializationOptions` extension `SetGameId` to manually specify a game ID when initializing mediation.

### Changed
- Overhauled the Mediation Settings UI:
    - Uninstalled indicators
    - Alternating backgrounds
    - Game ID display for game IDs retrieved from the Dashboard
- In-Editor Test Ads: Color removed from console logs

### Fixed
- Archived Ad Units no longer display in the Ad Units list.
- Fixed an issue where in-Editor Test Ads would not initialize if the build target was not supported by mediation.
- Removed an error that occurred when importing the play services resolver for the first time.
