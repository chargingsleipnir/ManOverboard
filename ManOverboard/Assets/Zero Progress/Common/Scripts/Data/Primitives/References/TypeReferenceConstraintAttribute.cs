// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{

    /// <summary>
    /// Indicates how selectable classes should be collated in drop-down menu.
    /// </summary>
    public enum ClassGrouping
    {
        /// <summary>
        /// No grouping, just show type names in a list; for instance, "Some.Nested.Namespace.SpecialClass".
        /// </summary>
        None,
        /// <summary>
        /// Group classes by namespace and show foldout menus for nested namespaces; for
        /// instance, "Some > Nested > Namespace > SpecialClass".
        /// </summary>
        ByNamespace,
        /// <summary>
        /// Group classes by namespace; for instance, "Some.Nested.Namespace > SpecialClass".
        /// </summary>
        ByNamespaceFlat,
        /// <summary>
        /// Group classes in the same way as Unity does for its component menu. This
        /// grouping method must only be used for <see cref="MonoBehaviour"/> types.
        /// </summary>
        ByAddComponentMenu,
        /// <summary>
        /// Group classes using custom logic
        /// </summary>
        Custom
    }

    /// <summary>
    /// Base class for class selection constraints that can be applied when selecting
    /// a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    public abstract class ClassTypeConstraintAttribute : PropertyAttribute
    {
        public delegate bool FilterTypeDelegate(System.Type type);
        public delegate string CustomGroupDelegate(System.Type type);

        private ClassGrouping _grouping = ClassGrouping.ByNamespaceFlat;
        private CustomGroupDelegate _customGroupingLogic = null;

        private bool _allowAbstract = false;
        private bool _allowStructs = false;
        private bool _allowGenerics = true;

        private bool _removeNamespaceFromSelected = false;
        private bool _showNoneOption = true;
        private FilterTypeDelegate _additionalFiltering = null;

        private bool _removeIfCustomDisplay = true;
        private Dictionary<Type, string> _customTypeDisplays = new Dictionary<Type, string>();
        
        /// <summary>
        /// Gets or sets grouping of selectable classes. Defaults to <see cref="ClassGrouping.ByNamespaceFlat"/>
        /// unless explicitly specified.
        /// </summary>
        public ClassGrouping Grouping
        {
            get { return _grouping; }
            set { _grouping = value; }
        }

        /// <summary>
        /// Gets or sets the custom grouping of selectable classes. 
        /// Defaults to null unless explicitly specified. Only used if
        /// <see cref="Grouping"/> is set to <see cref="ClassGrouping"/> is set to custom
        /// </summary>
        public CustomGroupDelegate CustomGroupingLogic
        {
            get { return _customGroupingLogic; }
            set { _customGroupingLogic = value; }
        }

        /// <summary>
        /// Gets or sets whether abstract classes can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowAbstract
        {
            get { return _allowAbstract; }
            set { _allowAbstract = value; }
        }

        /// <summary>
        /// Gets or sets whether structs can be selected from drop-down.
        /// Defaults to a value of <c>false</c> unless explicitly specified.
        /// </summary>
        public bool AllowStructs
        {
            get { return _allowStructs; }
            set { _allowStructs = value; }
        }

        /// <summary>
        /// Gets or sets whether generic classes can be selected from drop-down.
        /// Defaults to a value of <c>true</c> unless explicitly specified.
        /// </summary>
        public bool AllowGenerics
        {
            get { return _allowGenerics; }
            set { _allowGenerics = value; }
        }

        /// <summary>
        /// Gets or sets whether the selected item is displayed only
        /// as the class name, or if it includes the full namespace as well
        /// Defaults to a value of <c>false</c>
        /// </summary>
        public bool RemoveNamespaceFromSelected
        {
            get { return _removeNamespaceFromSelected; }
            set { _removeNamespaceFromSelected = value; }
        }

        /// <summary>
        /// Gets or sets whether an option for NONE exists.
        /// Don't use this if a selected type can be null
        /// 
        /// Defaults to a value of <c>true</c>
        /// </summary>
        public bool ShowNoneOption
        {
            get { return _showNoneOption; }
            set { _showNoneOption = value; }
        }

        /// <summary>
        /// Gets or sets additional filtering logic
        /// 
        /// Defaults to a value of <c>null</c>
        /// </summary>
        public FilterTypeDelegate AdditionalFiltering
        {
            get { return _additionalFiltering; }
            set { _additionalFiltering = value; }
        }

        /// <summary>
        /// Gets the dictionary to allow mapping a type to a custom display name
        /// </summary>
        public Dictionary<Type, string> CustomDisplays
        {
            get { return _customTypeDisplays; }
            private set { _customTypeDisplays = value; }
        }
        
        /// <summary>
        /// Remove the original type display if there is a custom
        /// display for it
        /// </summary>
        public bool RemoveIfHasCustomDisplay
        {
            get { return _removeIfCustomDisplay; }
            set { _removeIfCustomDisplay = value; }
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="Type"/> satisfies filter constraint.
        /// </summary>
        /// <param name="type">Type to test.</param>
        /// <returns>
        /// A <see cref="bool"/> value indicating if the type specified by <paramref name="type"/>
        /// satisfies this constraint and should thus be selectable.
        /// </returns>
        public virtual bool IsConstraintSatisfied(Type type)
        {
            if (!AllowAbstract && type.IsAbstract)
                return false;

            if (!AllowStructs && !type.IsClass)
                return false;

            if (!AllowGenerics && type.IsGenericTypeDefinition)
                return false;

            if (RemoveIfHasCustomDisplay && CustomDisplays.ContainsKey(type))
                return false;

            if (AdditionalFiltering != null && !AdditionalFiltering(type))
                return false;

            return true;
        }

    }

    /// <summary>
    /// Constraint that allows selection of classes that extend a specific class when
    /// selecting a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassExtendsAttribute : ClassTypeConstraintAttribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        public ClassExtendsAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassExtendsAttribute"/> class.
        /// </summary>
        /// <param name="baseType">Type of class that selectable classes must derive from.</param>
        public ClassExtendsAttribute(Type baseType)
        {
            BaseType = baseType;
        }

        /// <summary>
        /// Gets the type of class that selectable classes must derive from.
        /// </summary>
        public Type BaseType { get; private set; }

        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            return base.IsConstraintSatisfied(type)
                && BaseType.IsAssignableFrom(type) && type != BaseType;
        }

    }

    /// <summary>
    /// Constraint that allows selection of classes that implement a specific interface
    /// when selecting a <see cref="TypeReference"/> with the Unity inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ClassImplementsAttribute : ClassTypeConstraintAttribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        public ClassImplementsAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassImplementsAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of interface that selectable classes must implement.</param>
        public ClassImplementsAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        /// <summary>
        /// Gets the type of interface that selectable classes must implement.
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <inheritdoc/>
        public override bool IsConstraintSatisfied(Type type)
        {
            if (base.IsConstraintSatisfied(type))
            {
                foreach (var interfaceType in type.GetInterfaces())
                    if (interfaceType == InterfaceType)
                        return true;
            }
            return false;
        }

    }

}
