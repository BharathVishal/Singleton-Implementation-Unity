
# About Unity Advertisement with Mediation
The Unity Advertisement with Mediation package (com.unity.services.mediation) offers a centralized way to implement mediation so you can monetize your app with both the Unity Ads network and multiple third-party ad networks.

This package provides:

* Support in the Unity Editor to install, update, and manage your ad network adapters
* APIs to load and display interstitial and rewarded ads in your app
* Monetization strategies like mediation waterfalls to help maximize your ad revenue
* Support to work with iOS and Android apps
* Samples to help you get started

# Requirements
Unity Advertisement with Mediation is compatible with Unity Editor versions 2019.4 and later.

# Installing Unity Advertisement with Mediation
Open your Unity Editor and navigate to the Package Manager. Find the Advertisement with Mediation package and install it. Alternatively, add the package via a tgz file or from your disk if you downloaded it from the Unity Dashboard.

# Using Unity Advertisement with Mediation
After installing the Advertisement with Mediation package, follow these steps to start using Unity Mediation:

1. Create an organization in the [Unity Dashboard](https://dashboard.unity3d.com/monetization).
2. Contact your Unity representative to get access to Mediation in the dashboard.
3. Select **Unity Mediation** as the Ads Provider for each project you want to set up to use Mediation.
4. Set up your ad sources by configuring third-party ad networks in the Unity Dashboard.
5. In your Unity project, go to **Project Settings** > **Services** > **Mediation** to open the Mediation Configuration page and install the ad network adapters for each ad source you configured in the Unity Dashboard. Then, open the Code Generator to select a platform and an ad unit to generate a sample code snippet. Copy the snippet and add it to your scene in the game object’s `monobehaviour` to load and show an ad.  

For more information, refer to the following docs: 
* [Unity Mediation Setup Checklist](https://docs.unity.com/mediation/MediationSetupChecklist.html)
* [Ad Source Configuration guide](https://docs.unity.com/mediation/IntroAdSourceConfiguration.html)
* [Made with Unity Integration guide](https://docs.unity.com/mediation/SDKInstallConfigureUnity.html)
