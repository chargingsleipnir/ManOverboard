using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Stores a reference to an interface whether it is a unity scriptable object,
    /// monobehaviour, or plain old c# class
    /// </summary>
    /// <typeparam name="InterfaceType"></typeparam>
    [Serializable]
    public class InterfaceReference<InterfaceType> where InterfaceType : class
    {
        [SerializeField]
        [HideInInspector]
        protected UnityEngine.Object interfaceAsUnityObject = null;

        [SerializeField]
        [HideInInspector]
        protected SerializedPoco interfaceAsPoco = new SerializedPoco();

        /// <summary>
        /// Retrieves the wrapped interface value
        /// </summary>
        public InterfaceType Value
        {
            get
            {
                if (interfaceAsUnityObject != null)
                    return interfaceAsUnityObject as InterfaceType;

                if (interfaceAsPoco != null)
                    return interfaceAsPoco.ValueAs<InterfaceType>();

                return null;
            }
            set
            {
                SetInterfaceReference(value);
            }
        }
        
        /// <summary>
        /// Sets the interface value
        /// </summary>
        /// <param name="Interface">The instance to be set</param>
        public void SetInterfaceReference(InterfaceType Interface)
        {
            Reset();

            if (Interface is UnityEngine.Object)
                interfaceAsUnityObject = Interface as UnityEngine.Object;
            else
                interfaceAsPoco.SetValue(Interface);
        }

        /// <summary>
        /// Resets this reference to null
        /// </summary>
        public void Reset()
        {
            if (interfaceAsPoco != null)
                interfaceAsPoco.SetValue(null);

            interfaceAsUnityObject = null;
        }
    }         
}