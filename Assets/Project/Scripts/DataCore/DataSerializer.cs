using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using LWFlo.Constants;
using LWFlo.Tools;

namespace LWFlo
{
    public static class DataSerializer
    {
       public static byte[] EncryptData(this GameData gameData)
        {
            var iv = new byte[16];
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(LocalData.LOCAL_DATA_KEY);
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                var type = typeof(GameData);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                foreach (var currentField in fields)
                {
                    var fieldValue = currentField.GetValue(gameData);

                    if (fieldValue.IsNotNull() == true)
                    {
                        var relatedType = currentField.FieldType;
                        var relatedFields = relatedType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                        foreach (var currentRelatedField in relatedFields)
                        {
                            var relatedFieldValue = currentRelatedField.GetValue(fieldValue);
                            var composedField = relatedFieldValue + LocalData.DATA_SEPARATOR;
                            streamWriter.Write(composedField);
                        }
                    }
                }
            }
                        
            var array = memoryStream.ToArray();
            return array;
        }

        public static GameData DecryptData(this byte[] dataBytes)
        {
            var iv = new byte[16];
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(LocalData.LOCAL_DATA_KEY);
            aes.IV = iv;
            
            using var decrypted = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(dataBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decrypted, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            
            var decryptedString = streamReader.ReadToEnd();
            var fieldValues = decryptedString.Split(LocalData.DATA_SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
            var gameDataType = typeof(GameData);
            var gameDataFields = gameDataType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var gameData = Activator.CreateInstance<GameData>();
            var index = 0;
            foreach (var field in gameDataFields)
            {
                var fieldType = field.FieldType;
                if (fieldType.IsClass && fieldType != typeof(string))
                {
                    var nestedObject = Activator.CreateInstance(fieldType);
                    var nestedFields = fieldType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                    foreach (var nestedField in nestedFields)
                    {
                        if (index >= fieldValues.Length) 
                            break;
                        
                        var nestedValue = Convert.ChangeType(fieldValues[index++], nestedField.FieldType);
                        nestedField.SetValue(nestedObject, nestedValue);
                    }

                    field.SetValue(gameData, nestedObject);
                }
                else
                {
                    if (index >= fieldValues.Length) 
                        break;
                    
                    var fieldValue = Convert.ChangeType(fieldValues[index++], fieldType);
                    field.SetValue(gameData, fieldValue);
                }
            }

            return gameData;
        }
    }
}