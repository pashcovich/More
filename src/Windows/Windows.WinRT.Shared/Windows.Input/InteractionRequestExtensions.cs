﻿namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Linq;
    using global::Windows.ApplicationModel.Contacts;

    /// <summary>
    /// Provides extension methods for the <see cref="InteractionRequest{T}"/> class.
    /// </summary>
    public static class InteractionRequestExtensions
    {
        private static readonly IReadOnlyList<ContactFieldType> DefaultDesiredFields = new ContactFieldType[0];
        private static readonly IList<Contact> NoContacts = new Contact[0];

        /// <summary>
        /// Requests a single contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing selected <see cref="Contact">contact</see> or <c>null</c>
        /// if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        [CLSCompliant( false )]
        public static Task<Contact> RequestSingleContactAsync( this InteractionRequest<SelectContactInteraction> interactionRequest ) =>
            interactionRequest.RequestSingleContactAsync( null, DefaultDesiredFields );

        /// <summary>
        /// Requests a single contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="desiredFields">An <see cref="Array">array</see> of <see cref="ContactFieldType">contact field types</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing selected <see cref="Contact">contact</see> or <c>null</c>
        /// if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic parameter." )]
        public static Task<Contact> RequestSingleContactAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, params ContactFieldType[] desiredFields ) =>
            interactionRequest.RequestSingleContactAsync( null, desiredFields );

        /// <summary>
        /// Requests a single contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="selectButtonText">The select button text. The default value is "Select".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing selected <see cref="Contact">contact</see> or <c>null</c>
        /// if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "Select" and "Cancel"</remarks>
        [CLSCompliant( false )]
        public static Task<Contact> RequestSingleContactAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, string selectButtonText ) =>
            interactionRequest.RequestSingleContactAsync( selectButtonText, DefaultDesiredFields );

        /// <summary>
        /// Requests a single contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="selectButtonText">The select button text. The default value is "Select".</param>
        /// <param name="desiredFields">A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="ContactFieldType">contact field types</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing selected <see cref="Contact">contact</see> or <c>null</c>
        /// if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "Select" and "Cancel"</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static Task<Contact> RequestSingleContactAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, string selectButtonText, IReadOnlyList<ContactFieldType> desiredFields )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( desiredFields, nameof( desiredFields ) );

            if ( string.IsNullOrEmpty( selectButtonText ) )
                selectButtonText = SR.SelectCaption;

            var source = new TaskCompletionSource<Contact>();
            SelectContactInteraction interaction = null;

            interaction = new SelectContactInteraction()
            {
                Multiselect = false,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Select", selectButtonText, p => source.TrySetResult( interaction.Contacts.FirstOrDefault() ) ),
                    new NamedCommand<object>( "Cancel", SR.CancelCaption, p => source.TrySetResult( null ) )
                }
            };

            interaction.DesiredFields.AddRange( desiredFields );
            interactionRequest.Request( interaction );

            return source.Task;
        }

        /// <summary>
        /// Requests a multiple contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="Contact">contacts</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic parameter." )]
        public static Task<IList<Contact>> RequestMultipleContactsAsync( this InteractionRequest<SelectContactInteraction> interactionRequest ) =>
            interactionRequest.RequestMultipleContactsAsync( null, DefaultDesiredFields );

        /// <summary>
        /// Requests a multiple contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="desiredFields">An <see cref="Array">array</see> of <see cref="ContactFieldType">contact field types</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="Contact">contacts</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers and names "Select" and "Cancel", respectively.</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic parameter." )]
        public static Task<IList<Contact>> RequestMultipleContactsAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, params ContactFieldType[] desiredFields ) =>
            interactionRequest.RequestMultipleContactsAsync( null, desiredFields );

        /// <summary>
        /// Requests a multiple contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="selectButtonText">The select button text. The default value is "Select".</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="Contact">contacts</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "Select" and "Cancel"</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic parameter." )]
        public static Task<IList<Contact>> RequestMultipleContactsAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, string selectButtonText ) =>
            interactionRequest.RequestMultipleContactsAsync( selectButtonText, DefaultDesiredFields );

        /// <summary>
        /// Requests a multiple contact selection interaction asynchronously.
        /// </summary>
        /// <param name="interactionRequest">The extended <see cref="InteractionRequest{T}">interaction request</see>.</param>
        /// <param name="selectButtonText">The select button text. The default value is "Select".</param>
        /// <param name="desiredFields">A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="ContactFieldType">contact field types</see>.</param>
        /// <returns>A <see cref="Task{TResult}">task</see> containing a <see cref="IList{T}">list</see> of the selected <see cref="Contact">contacts</see>.
        /// The list is empty if the operation was canceled.</returns>
        /// <remarks>The requested <see cref="SelectContactInteraction">interaction</see> will always contain two <see cref="INamedCommand">commands</see>
        /// with the identifiers "Select" and "Cancel"</remarks>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for Task<T> with a generic parameter." )]
        public static Task<IList<Contact>> RequestMultipleContactsAsync( this InteractionRequest<SelectContactInteraction> interactionRequest, string selectButtonText, IReadOnlyList<ContactFieldType> desiredFields )
        {
            Arg.NotNull( interactionRequest, nameof( interactionRequest ) );
            Arg.NotNull( desiredFields, nameof( desiredFields ) );

            if ( string.IsNullOrEmpty( selectButtonText ) )
                selectButtonText = SR.SelectCaption;

            var source = new TaskCompletionSource<IList<Contact>>();
            SelectContactInteraction interaction = null;

            interaction = new SelectContactInteraction()
            {
                Multiselect = true,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 1,
                Commands =
                {
                    new NamedCommand<object>( "Select", selectButtonText, p => source.TrySetResult( interaction.Contacts ) ),
                    new NamedCommand<object>( "Cancel", SR.CancelCaption, p => source.TrySetResult( NoContacts ) )
                }
            };

            interaction.DesiredFields.AddRange( desiredFields );
            interactionRequest.Request( interaction );

            return source.Task;
        }
    }
}
