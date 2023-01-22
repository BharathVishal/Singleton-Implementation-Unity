#if UNITY_ANDROID
using System.Xml;
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor.Android;

namespace Unity.Services.Mediation.Build.Editor
{
    class AndroidManifestPostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        /*
         * This Utility will be used to inject Application Id (and other properties) to the AndroidManifest file.
         */

        const string k_GmsApplicationIdName = "com.google.android.gms.ads.APPLICATION_ID";

        const string k_AndroidManifestPath = "/src/main/AndroidManifest.xml";

        const string k_AndroidURI = "http://schemas.android.com/apk/res/android";

        public int callbackOrder { get; }


        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var adMobSettings = new AdMobSettings();
            //If we're not including AdMob, no need to modify AndroidManifest.xml
            if (string.IsNullOrEmpty(adMobSettings.InstalledVersion.value) || string.IsNullOrWhiteSpace(adMobSettings.AdMobAppIdAndroid))
                return;

            string manifestPath = path + k_AndroidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);

            var manifestNode = FindFirstChild(manifestDoc, "manifest");
            if (manifestNode == null)
                return;

            var applicationNode = FindFirstChild(manifestNode, "application");
            if (applicationNode == null)
                return;

            FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data",
                "name", k_GmsApplicationIdName,
                "value", adMobSettings.AdMobAppIdAndroid);

            manifestDoc.Save(manifestPath);
        }

        XmlNode FindFirstChild(XmlNode node, string tag)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; ++i)
                {
                    var child = node.ChildNodes[i];
                    if (child.Name == tag)
                        return child;
                }
            }

            return null;
        }

        void FindOrCreateTagWithAttributes(XmlDocument doc, XmlNode containingNode, string tagName,
            string firstAttributeName, string firstAttributeValue, string secondAttributeName, string secondAttributeValue)
        {
            if (containingNode.HasChildNodes)
            {
                for (int i = 0; i < containingNode.ChildNodes.Count; ++i)
                {
                    var childNode = containingNode.ChildNodes[i];
                    if (childNode.Name == tagName)
                    {
                        var childElement = childNode as XmlElement;
                        if (childElement != null && childElement.HasAttributes)
                        {
                            var firstAttribute = childElement.GetAttributeNode(firstAttributeName, k_AndroidURI);
                            if (firstAttribute == null || firstAttribute.Value != firstAttributeValue)
                                continue;

                            var secondAttribute = childElement.GetAttributeNode(secondAttributeName, k_AndroidURI);
                            if (secondAttribute != null)
                            {
                                secondAttribute.Value = secondAttributeValue;
                                return;
                            }

                            // Create it
                            AppendNewAttribute(doc, childElement, secondAttributeName, secondAttributeValue);
                            return;
                        }
                    }
                }
            }

            // Didn't find it, so create it
            var element = doc.CreateElement(tagName);
            AppendNewAttribute(doc, element, firstAttributeName, firstAttributeValue);
            AppendNewAttribute(doc, element, secondAttributeName, secondAttributeValue);
            containingNode.AppendChild(element);
        }

        void AppendNewAttribute(XmlDocument doc, XmlElement element, string attributeName, string attributeValue)
        {
            var attribute = doc.CreateAttribute(attributeName, k_AndroidURI);
            attribute.Value = attributeValue;
            element.Attributes.Append(attribute);
        }
    }
}
#endif
