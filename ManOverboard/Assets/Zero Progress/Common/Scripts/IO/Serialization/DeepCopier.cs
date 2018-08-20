using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Provides utilities for performing Deep Copies of objects
/// </summary>
public static class DeepCopier {

    /// <summary>
    /// Extension method for performing a deep copy on the specified object. 
    /// Not the most performant (performs serialization and deserialization)
    /// </summary>
    /// <typeparam name="T">The type of object to be deep copied. Must be marked [Serializable]</typeparam>
    /// <param name="SourceObject">The instance to be deep copied</param>
    /// <returns>The deep copied instance, or default(T) if the provided object is null</returns>
    /// <exception cref="ArgumentException">Thrown if the provided SourceObject is not of a [Serializable] type</exception>
	public static T DeepCopy<T>(this T SourceObject)
    {
        if (!typeof(T).IsSerializable)
            throw new ArgumentException("The type of the SourceObject is not serializable. Cannot perform Deep Copy", "SourceObject");

        if (ReferenceEquals(SourceObject, null))
            return default(T);

        return PerformDeepCopy(SourceObject);
    }

    /// <summary>
    /// The logic behind performing the deep copy, abstracted out in case it needs to be used in
    /// overloads or other actions this utility will provide
    /// </summary>
    /// <typeparam name="T">The type of object to be deep copied. Must be marked [Serializable]</typeparam>
    /// <param name="ObjectToCopy">The instance to be deep copied. Must not be null</param>
    /// <returns>The deep copied instance</returns>
    private static T PerformDeepCopy<T>(T ObjectToCopy)
    {
        IFormatter formatter = new BinaryFormatter();

        using (Stream stream = new MemoryStream())
        {
            formatter.Serialize(stream, ObjectToCopy);

            // Move to beginning of the stream before deserializing
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
