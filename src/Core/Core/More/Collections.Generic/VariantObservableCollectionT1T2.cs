﻿namespace More.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Represents a covariant and contravariant version of the <see cref="ObservableCollection{T}"/> class.
    /// </summary>
    /// <typeparam name="TFrom">The <see cref="Type">type</see> to make covariant.</typeparam>
    /// <typeparam name="TTo">The <see cref="Type">type</see> of contravariant item.</typeparam>
    [DebuggerDisplay( "Count = {Count}" )]
    [DebuggerTypeProxy( typeof( CollectionDebugView<> ) )]
    public class VariantObservableCollection<TFrom, TTo> : ObservableCollection<TTo>, IList<TFrom> where TFrom : TTo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariantObservableCollection{TFrom,TTo}"/> class.
        /// </summary>
        public VariantObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariantObservableCollection{TFrom,TTo}"/> class.
        /// </summary>
        /// <param name="sequence">A <see cref="IEnumerable{T}">sequence</see> of items to initially make covariant.</param>
        public VariantObservableCollection( IEnumerable<TFrom> sequence )
            : base( sequence == null ? Enumerable.Empty<TTo>() : sequence.Cast<TTo>() )
        {
            Arg.NotNull( sequence, nameof( sequence ) );
        }

        int IList<TFrom>.IndexOf( TFrom item )
        {
            return IndexOf( (TTo) item );
        }

        void IList<TFrom>.Insert( int index, TFrom item )
        {
            InsertItem( index, (TTo) item );
        }

        void IList<TFrom>.RemoveAt( int index )
        {
            RemoveAt( index );
        }

        TFrom IList<TFrom>.this[int index]
        {
            get
            {
                return (TFrom) this[index];
            }
            set
            {
                SetItem( index, (TTo) value );
            }
        }

        void ICollection<TFrom>.Add( TFrom item )
        {
            InsertItem( Count, (TTo) item );
        }

        void ICollection<TFrom>.Clear()
        {
            ClearItems();
        }

        bool ICollection<TFrom>.Contains( TFrom item )
        {
            return Contains( (TTo) item );
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive" )]
        void ICollection<TFrom>.CopyTo( TFrom[] array, int arrayIndex )
        {
            Arg.NotNull( array, nameof( array ) );
            var other = new TTo[array.Length];
            Items.CopyTo( other, arrayIndex );
            other.Cast<TFrom>().ToList().CopyTo( array, arrayIndex );
        }

        int ICollection<TFrom>.Count
        {
            get
            {
                return Count;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Hidden covariant implementation." )]
        bool ICollection<TFrom>.IsReadOnly
        {
            get
            {
                return ( (ICollection<TTo>) this ).IsReadOnly;
            }
        }

        bool ICollection<TFrom>.Remove( TFrom item )
        {
            return Remove( (TTo) item );
        }

        IEnumerator<TFrom> IEnumerable<TFrom>.GetEnumerator()
        {
            return Items.Cast<TFrom>().GetEnumerator();
        }
    }
}
