﻿namespace More
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an object for comparing <see cref="Uri"/> objects.
    /// </summary>
    public class UriComparer : IComparer<Uri>, IComparer, IEqualityComparer<Uri>, IEqualityComparer
    {
        private static readonly UriComparer ordinal = new UriComparer( UriComponents.AbsoluteUri, UriFormat.Unescaped, false );
        private static readonly UriComparer ordinalIgnoreCase = new UriComparer( UriComponents.AbsoluteUri, UriFormat.Unescaped, true );
        private readonly StringComparison comparison = StringComparison.OrdinalIgnoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="UriComparer"/> class.
        /// </summary>
        /// <remarks>This constructor uses <see cref="T:UriComponents.AbsoluteUri"/>, <see cref="T:UriFormat.Unescaped"/>,
        /// and is not case sensitive.</remarks>
        public UriComparer()
            : this( UriComponents.AbsoluteUri, UriFormat.Unescaped, true )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriComparer"/> class.
        /// </summary>
        /// <param name="components">One or more of the <see cref="UriComponents"/> that are compared.</param>
        /// <param name="format">The <see cref="UriFormat"/> used in comparisons.</param>
        /// <remarks>This constructor is not case sensitive.</remarks>
        public UriComparer( UriComponents components, UriFormat format )
            : this( components, format, true )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriComparer"/> class.
        /// </summary>
        /// <param name="components">One or more of the <see cref="UriComponents"/> that are compared.</param>
        /// <param name="format">The <see cref="UriFormat"/> used in comparisons.</param>
        /// <param name="ignoreCase">Indicates whether comparisons are case sensitive.</param>
        public UriComparer( UriComponents components, UriFormat format, bool ignoreCase )
        {
            UriComponents = components;
            UriFormat = format;
            comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        /// <summary>
        /// Gets the default ordinal comparer.
        /// </summary>
        /// <value>A <see cref="UriComparer"/> object.</value>
        /// <remarks>This property returns a comparer that evaluates <see cref="Uri"/> objects based on their
        /// unescaped, <see cref="P:Uri.AbsoluteUri">absolute URI</see> properties with case sensitivity.</remarks>
        public static UriComparer Ordinal
        {
            get
            {
                Contract.Ensures( Contract.Result<UriComparer>() != null ); 
                return ordinal;
            }
        }

        /// <summary>
        /// Gets the default ordinal comparer without case sensitivity.
        /// </summary>
        /// <value>A <see cref="UriComparer"/> object.</value>
        /// <remarks>This property returns a comparer that evaluates <see cref="Uri"/> objects based on their
        /// unescaped, <see cref="P:Uri.AbsoluteUri">absolute URI</see> properties with case insensitivity.</remarks>
        public static UriComparer OrdinalIgnoreCase
        {
            get
            {
                Contract.Ensures( Contract.Result<UriComparer>() != null ); 
                return ordinalIgnoreCase;
            }
        }

        /// <summary>
        /// Gets the components the comparer evaluates.
        /// </summary>
        /// <value>One or more of the <see cref="UriComponents"/>.</value>
        public UriComponents UriComponents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the format the comparer evaluates.
        /// </summary>
        /// <value>One of the <see cref="UriFormat"/> values.</value>
        public UriFormat UriFormat
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the comparer is case sensitive.
        /// </summary>
        /// <value>True if the comparer is case sensitive; otherwise, false.</value>
        public bool IgnoreCase
        {
            get
            {
                return comparison == StringComparison.OrdinalIgnoreCase;
            }
        }

        /// <summary>
        /// Compares two <see cref="Uri"/> objects.
        /// </summary>
        /// <param name="x">The <see cref="Uri"/> object that is the basis of the comparison.</param>
        /// <param name="y">The <see cref="Uri"/> object to compare against.</param>
        /// <returns>Zero if the two objects are equal, one if <paramref name="x"/> is greater than <paramref name="y"/>, or
        /// negative one if <paramref name="x"/> is less than <paramref name="y"/>.</returns>
        public virtual int Compare( Uri x, Uri y )
        {
            return Uri.Compare( x, y, UriComponents, UriFormat, comparison );
        }

        int IComparer.Compare( object x, object y )
        {
            return Compare( (Uri) x, (Uri) y );
        }

        /// <summary>
        /// Compares the equality of two <see cref="Uri"/> objects.
        /// </summary>
        /// <param name="x">The <see cref="Uri"/> object that is the basis of the comparison.</param>
        /// <param name="y">The <see cref="Uri"/> object to compare against.</param>
        /// <returns>True if the two objects are equal; otherwise, false.</returns>
        public virtual bool Equals( Uri x, Uri y )
        {
            return Compare( x, y ) == 0;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="Uri"/> object to get a hash code for.</param>
        /// <returns>A hash code.</returns>
        /// <remarks>This method returns the default implementation of <see cref="M:Uri.GetHashCode"/>.</remarks>
        public virtual int GetHashCode( Uri obj )
        {
            return obj == null ? 0 : obj.GetHashCode();
        }

        bool IEqualityComparer.Equals( object x, object y )
        {
            return Equals( (Uri) x, (Uri) y );
        }

        int IEqualityComparer.GetHashCode( object obj )
        {
            return GetHashCode( (Uri) obj );
        }
    }
}
