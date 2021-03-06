﻿namespace More.Windows.Data
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
#if NETFX_CORE
    using global::Windows.UI.Xaml.Data;
#endif

    /// <summary>
    /// Defines the behavior of a collection view that contains frozen items.
    /// </summary>
#if NETFX_CORE
    [CLSCompliant( false )]
#endif
    [ContractClass( typeof( IFrozenItemCollectionViewContract ) )]
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This a view of a collection, not a collection itself." )]
    public interface IFrozenItemCollectionView : ICollectionView
    {
        /// <summary>
        /// Gets or sets the position of frozen items in the collection view.
        /// </summary>
        /// <value>One of the <see cref="FrozenItemPosition"/> values.</value>
        FrozenItemPosition FrozenItemPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a sequence of frozen items in the collection view.
        /// </summary>
        /// <value>An <see cref="IEnumerable"/> object.</value>
        IEnumerable FrozenItems
        {
            get;
        }

        /// <summary>
        /// Gets a sequence of unfrozen items in the collection view.
        /// </summary>
        /// <value>An <see cref="IEnumerable"/> object.</value>
        IEnumerable UnfrozenItems
        {
            get;
        }
    }
}
