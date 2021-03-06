﻿namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a range-based validation rule for <see cref="Nullable{T}">nullable</see> types.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of value to validate.</typeparam>
    public class NullableRangeRule<T> : IRule<Property<T?>, IValidationResult> where T : struct, IComparable<T>
    {
        private readonly T minimum;
        private readonly T maximum;
        private readonly string errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRangeRule{T}"/> class.
        /// </summary>
        /// <param name="maximum">The maximum range value.</param>
        /// <remarks>The <see cref="P:Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public NullableRangeRule( T maximum )
        {
            Arg.GreaterThanOrEqualTo( maximum, default( T ), "maximum" );

            this.maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRangeRule{T}"/> class.
        /// </summary>
        /// <param name="maximum">The maximum range value.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        /// <remarks>The <see cref="P:Minimum"/> range value is the default value of <typeparamref name="T"/>.</remarks>
        public NullableRangeRule( T maximum, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( maximum, default( T ), "maximum" );

            this.maximum = maximum;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRangeRule{T}"/> class.
        /// </summary>
        /// <param name="minimum">The minimum range value.</param>
        /// <param name="maximum">The maximum range value.</param>
        public NullableRangeRule( T minimum, T maximum )
        {
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );

            this.minimum = minimum;
            this.maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullableRangeRule{T}"/> class.
        /// </summary>
        /// <param name="minimum">The minimum range value.</param>
        /// <param name="maximum">The maximum range value.</param>
        /// <param name="errorMessage">The error message associated with the rule.</param>
        public NullableRangeRule( T minimum, T maximum, string errorMessage )
        {
            Arg.NotNullOrEmpty( errorMessage, nameof( errorMessage ) );
            Arg.GreaterThanOrEqualTo( maximum, minimum, "maximum" );

            this.minimum = minimum;
            this.maximum = maximum;
            this.errorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the minimum range value.
        /// </summary>
        /// <value>The minimum range value.</value>
        public T Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum range value.
        /// </summary>
        /// <value>THe maximum range value.</value>
        public T Maximum
        {
            get
            {
                Contract.Ensures( Contract.Result<T>().CompareTo( Minimum ) >= 0 );
                return maximum;
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified item.
        /// </summary>
        /// <param name="item">The <see cref="Property{T}">property</see> to validate.</param>
        /// <returns>The <see cref="IValidationResult">validation result</see> of the evaluation.</returns>
        public virtual IValidationResult Evaluate( Property<T?> item )
        {
            if ( item == null || item.Value == null )
                return ValidationResult.Success;

            var value = item.Value.Value;

            if ( value.CompareTo( Minimum ) < 0 || value.CompareTo( Maximum ) > 0 )
            {
                var message = errorMessage ?? ValidationMessage.RangeValidationError.FormatDefault( item.Name, Minimum, Maximum );
                return new ValidationResult( message, item.Name );
            }

            return ValidationResult.Success;
        }
    }
}
