﻿namespace More.Windows.Media
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading.Tasks;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Provides extension methods for the <see cref="MediaContent{T}"/> class.
    /// </summary>
    public static class MediaContentTExtensions
    {
        /// <summary>
        /// Returns the media content from the specified embedded resource asynchronously.
        /// </summary>
        /// <param name="content">The extended <see cref="MediaContent{T}">media content</see>.</param>
        /// <param name="resourceName">The name of the embedded resource to retrieve.</param>
        /// <returns>A <see cref="Task{T}">task</see> containing an object of type <typeparamref name="TMedia"/>.</returns>
        /// <remarks>The specified <paramref name="resourceName">resource name</paramref> is resolved by searching in
        /// the <see cref="M:Assembly.CallingAssembly">calling assembly</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<TMedia> FromEmbeddedResourceAsync<TMedia>( this MediaContent<TMedia> content, string resourceName )
        {
            Arg.NotNull( content, nameof( content ) );
            Arg.NotNullOrEmpty( resourceName, nameof( resourceName ) );
            Contract.Ensures( Contract.Result<Task<TMedia>>() != null );

            var application = Application.Current;

            if ( application == null )
                return Task.FromResult( default( TMedia ) );

            var assembly = application.GetType().GetTypeInfo().Assembly;
            return content.FromEmbeddedResourceAsync( assembly, resourceName );
        }
    }
}
