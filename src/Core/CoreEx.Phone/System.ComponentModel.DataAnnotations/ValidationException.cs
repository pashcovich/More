﻿namespace System.ComponentModel.DataAnnotations
{
    using global::System;

    /// <summary>
    /// Represents the exception that occurs during validation of a data field when the <see cref="ValidationAttribute"/> class is used.
    /// </summary>
    /// <remarks>This class provides ported compatibility for System.ComponentModel.DataAnnotations.ValidationException.</remarks>
    public class ValidationException : Exception
    {
        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" /> class using an error message generated by the system.
        /// </summary>
        public ValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" /> class using a specified error message.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        public ValidationException( string message )
            : base( message )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" /> class using a specified error message and a collection of inner exception instances.
        /// </summary>
        /// <param name="message">A specified message that states the error.</param>
        /// <param name="innerException">The collection of validation exceptions.</param>
        public ValidationException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" /> class by using a validation result, a validation attribute, and the value of the current exception.
        /// </summary>
        /// <param name="validationResult">The list of validation results.</param>
        /// <param name="validatingAttribute">The attribute that caused the current exception.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger the validation error.</param>
        public ValidationException( ValidationResult validationResult, ValidationAttribute validatingAttribute, object value )
            : this( validationResult.ErrorMessage, validatingAttribute, value )
        {
            this.validationResult = validationResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException" /> class using a specified error message, a validation attribute, and the value of the current exception.
        /// </summary>
        /// <param name="errorMessage">The message that states the error.</param>
        /// <param name="validatingAttribute">The attribute that caused the current exception.</param>
        /// <param name="value">The value of the object that caused the attribute to trigger validation error.</param>
        public ValidationException( string errorMessage, ValidationAttribute validatingAttribute, object value )
            : base( errorMessage )
        {
            this.Value = value;
            this.ValidationAttribute = validatingAttribute;
        }

        /// <summary>
        /// Gets the instance of the <see cref="ValidationAttribute" /> class that triggered this exception.
        /// </summary>
        /// <value>An instance of the validation attribute type that triggered this exception.</value>
        public ValidationAttribute ValidationAttribute
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="ValidationResult" /> instance that describes the validation error.
        /// </summary>
        /// <value>The <see cref="ValidationResult" /> instance that describes the validation error.</value>
        public ValidationResult ValidationResult
        {
            get
            {
                return this.validationResult ?? ( this.validationResult = new ValidationResult( this.Message ) );
            }
        }

        /// <summary>
        /// Gets the value of the object that causes the <see cref="ValidationAttribute" /> class to trigger this exception.
        /// </summary>
        /// <value>The value of the object that caused the <see cref="ValidationAttribute" /> class to trigger the validation error.</value>
        public object Value
        {
            get;
            private set;
        }
    }
}