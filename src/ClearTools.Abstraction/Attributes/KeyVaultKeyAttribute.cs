using System;

namespace ClearTools.Abstraction.Attributes
{
    /// <summary>
    /// Attribute to specify the Key Vault key name for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyVaultKeyAttribute : Attribute
    {
        public string KeyName { get; }

        public KeyVaultKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }
}
