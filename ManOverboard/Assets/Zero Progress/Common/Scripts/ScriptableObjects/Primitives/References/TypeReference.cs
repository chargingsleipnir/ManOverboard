// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Reference to a class <see cref="System.Type"/> with support for Unity serialization.
    /// </summary>
    [Serializable]
    public sealed class TypeReference : ISerializationCallbackReceiver, IEquatable<TypeReference>, IEquatable<Type>
    {
        /// <summary>
        /// Gets the string representation of the type
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns>The string representation of the class ref</returns>
        public static string GetClassRef(Type type)
        {
            return type != null
                ? type.FullName + ", " + type.Assembly.GetName().Name
                : "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        public TypeReference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="assemblyQualifiedClassName">Assembly qualified class name.</param>
        public TypeReference(string assemblyQualifiedClassName)
        {
            Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
                ? Type.GetType(assemblyQualifiedClassName)
                : null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="type">Class type.</param>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="type"/> is not a class type.
        /// </exception>
        public TypeReference(Type type)
        {
            Type = type;
        }

        [SerializeField]
        private string _classRef;

        #region ISerializationCallbackReceiver Members

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(_classRef))
            {
                _type = System.Type.GetType(_classRef);

                if (_type == null)
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", _classRef));
            }
            else
            {
                _type = null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        #endregion

        private Type _type;

        /// <summary>
        /// Gets or sets type of class reference.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// If <paramref name="value"/> is not a class type.
        /// </exception>
        public Type Type
        {
            get { return _type; }
            set
            {
                if (value != null && !value.IsClass)
                    throw new ArgumentException(string.Format("'{0}' is not a class type.", value.FullName), "value");

                _type = value;
                _classRef = GetClassRef(value);
            }
        }

        public static implicit operator string(TypeReference typeReference)
        {
            return typeReference._classRef;
        }

        public static implicit operator Type(TypeReference typeReference)
        {
            return typeReference.Type;
        }

        public static implicit operator TypeReference(Type type)
        {
            return new TypeReference(type);
        }

        /// <summary>
        /// Equals implementation to be able to compare against another TypeReference 
        /// or Type
        /// </summary>
        /// <param name="obj">The other TypeReference or Type to check</param>
        /// <returns>True if the passed object is equal to this one, false if not</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if(obj is TypeReference || obj is Type)
                return Equals((Type)obj);

            return false;
        }

        /// <summary>
        /// Gets the HashCode for this type
        /// </summary>
        /// <returns>The hash code value</returns>
        public override int GetHashCode()
        {
            if (Type == null)
                return 0;

            return Type.GetHashCode();
        }

        /// <summary>
        /// Equals implementation to be able to compare against another TypeReference 
        /// or Type
        /// </summary>
        /// <param name="obj">The other TypeReference to check</param>
        /// <returns>True if the passed object is equal to this one, false if not</returns>
        public bool Equals(TypeReference other)
        {
            if (other == null)
                return false;

            return Equals((Type)other);
        }

        /// <summary>
        /// Equals implementation to be able to compare against another Type 
        /// or Type
        /// </summary>
        /// <param name="obj">The Type to check</param>
        /// <returns>True if the passed object is equal to this one, false if not</returns>
        public bool Equals(Type other)
        {
            if (other == null)
                return false;

            return Type.Equals(other);
        }

        public static bool operator ==(TypeReference Ref1, TypeReference Ref2)
        {
            if (ReferenceEquals(Ref1, null))
                return false;

            return Ref1.Equals(Ref2);
        }

        public static bool operator !=(TypeReference Ref1, TypeReference Ref2)
        {
            if (ReferenceEquals(Ref1, null))
                return false;

            return !Ref1.Equals(Ref2);
        }

        public override string ToString()
        {
            return Type != null ? Type.FullName : "(None)";
        }
    }

}