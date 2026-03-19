using System;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Attribute to specify the App Configuration key name for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AppConfigurationKeyAttribute : Attribute
    {
        public string KeyName { get; }

        public AppConfigurationKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }

    /// <summary>
    /// Attribute to specify that a boolean property should be mapped to a feature flag
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FeatureFlagAttribute : Attribute
    {
        public string FlagName { get; }

        public FeatureFlagAttribute(string flagName)
        {
            FlagName = flagName;
        }
    }
}
