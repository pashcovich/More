﻿namespace More.ComponentModel
{
    using Collections.Generic;
    using DataAnnotations;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidatableObject" />.
    /// </summary>
    public class ValidatableObjectTest
    {
        public class MockValidatableObject : ValidatableObject
        {
            private int id;
            private string name;
            private string address;
            private DateTime hireDate = DateTime.Today;
            private DateTime? separationDate;

            public MockValidatableObject()
                : base( new Mock<IValidator>().Object )
            {
            }

            public MockValidatableObject( IValidator validator )
                : base( validator )
            {
            }

            public int Id
            {
                get
                {
                    return id;
                }
                set
                {
                    SetProperty( ref id, value );
                }
            }

            /// <summary>
            /// [Required]
            /// [StringLength( 50 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    SetProperty( ref name, value );
                }
            }

            /// <summary>
            /// [Required]
            /// [StringLength( 250 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
            public string Address
            {
                get
                {
                    return address;
                }
                set
                {
                    SetProperty( ref address, value );
                }
            }

            public DateTime HireDate
            {
                get
                {
                    return hireDate;
                }
                set
                {
                    SetProperty( ref hireDate, value );
                }
            }

            public DateTime? SeparationDate
            {
                get
                {
                    return separationDate;
                }
                set
                {
                    SetProperty( ref separationDate, value );
                }
            }

            public int DoWork() => default( int );

            public IMultivalueDictionary<string, IValidationResult> InvokeGetPropertyErrors() => PropertyErrors;

            public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue ) => IsPropertyValid( newValue, propertyName );

            public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue, ICollection<IValidationResult> results ) =>
                IsPropertyValid( newValue, results, propertyName );

            public void InvokeValidateProperty<TValue>( string propertyName, TValue newValue ) => ValidateProperty( newValue, propertyName );

            public IEnumerable<string> InvokeFormatErrorMessages( string propertyName, IEnumerable<IValidationResult> results ) =>
                FormatErrorMessages( propertyName, results );

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer ) =>
                SetProperty( ref currentValue, newValue, comparer, propertyName );

            public void InvokeOnErrorsChanged( string propertyName ) => OnErrorsChanged( propertyName );

            public void InvokeOnErrorsChanged( DataErrorsChangedEventArgs e ) => OnErrorsChanged( e );
        }

        [Fact( DisplayName = "set property should change property with comparison" )]
        public void SetPropertyShouldChangePropertyWithComparison()
        {
            // arrange
            var mockBackingField = "test";
            var propertyChanged = false;
            var errorsChanged = false;
            var expected = "TEST";
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );
            var context = new Mock<IValidationContext>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                               {
                                   r.Add( new Mock<IValidationResult>().Object );
                               }

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            target.PropertyChanged += ( s, e ) => propertyChanged = true;
            target.ErrorsChanged += ( s, e ) => errorsChanged = true;

            // act
            target.InvokeSetProperty( "Name", ref mockBackingField, "TEST", StringComparer.Ordinal );

            // assert
            Assert.True( propertyChanged );
            Assert.False( errorsChanged );
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 0, target.InvokeGetPropertyErrors().Count );
        }

        [Fact( DisplayName = "set property should propagate validation errors" )]
        public void SetPropertyShouldRaiseValidationErrors()
        {
            // arrange
            var mockBackingField = "test";
            var propertyChanged = false;
            var errorsChanged = false;
            var expected = string.Empty;
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );
            var context = new Mock<IValidationContext>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                               {
                                   r.Add( new Mock<IValidationResult>().Object );
                               }

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            target.PropertyChanged += ( s, e ) => propertyChanged = true;
            target.ErrorsChanged += ( s, e ) => errorsChanged = true;

            // act
            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // assert
            Assert.True( propertyChanged );
            Assert.True( errorsChanged );
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 1, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 1, target.InvokeGetPropertyErrors()["Name"].Count );
            Assert.False( target.IsValid );
        }

        [Fact( DisplayName = "set property should clear validation errors with valid value" )]
        public void SetPropertyShouldClearErrorsWhenPropertyIsCorrected()
        {
            // arrange
            var mockBackingField = "test";
            var expected = string.Empty;
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );
            var context = new Mock<IValidationContext>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                               {
                                   r.Add( new Mock<IValidationResult>().Object );
                               }

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // verify arrangement
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 1, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 1, target.InvokeGetPropertyErrors()["Name"].Count );

            // act
            expected = "TEST";
            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // assert
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 0, target.InvokeGetPropertyErrors().Count );
        }

        [Fact( DisplayName = "on errors changed should not allow null argument" )]
        public void OnErrorsChangedShouldNotAllowNullEventArgs()
        {
            // arrange
            DataErrorsChangedEventArgs e = null;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeOnErrorsChanged( e ) );

            // assert
            Assert.Equal( "e", ex.ParamName );
        }

        [Fact( DisplayName = "on errors changed should raise vent for property" )]
        public void OnErrorsChangedShouldRaiseEventByPropertyName()
        {
            // arrange
            var expected = "Test";
            var raised = false;
            var target = new MockValidatableObject();

            target.ErrorsChanged += ( s, e ) => raised = e.PropertyName.Equals( expected );

            // act
            target.InvokeOnErrorsChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on errors changed should raise event with argument" )]
        public void OnErrorsChangedShouldRaiseEventWithEventArgs()
        {
            // arrange
            var expected = new DataErrorsChangedEventArgs( "Test" );
            var raised = false;
            var target = new MockValidatableObject();

            target.ErrorsChanged += ( s, e ) => raised = e.Equals( expected );

            // act
            target.InvokeOnErrorsChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "is valid should return false for invalid object" )]
        public void IsValidPropertyShouldReturnFalseWhenNamePropertyIsUnset()
        {
            // arrange
            var result = new Mock<IValidationResult>();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );

            result.SetupGet( r => r.MemberNames ).Returns( new[] { "Test" } );
            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( context.Object );
            validator.Setup( v => v.TryValidateObject( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>(), It.IsAny<bool>() ) )
                     .Callback( ( object v, IValidationContext c, ICollection<IValidationResult> r, bool a ) => r.Add( result.Object ) )
                     .Returns( false );
            target.Validate();

            // act
            var valid = target.IsValid;

            // assert
            Assert.False( valid );
        }

        [Fact( DisplayName = "is property valid should not allow null or empty name" )]
        public void IsPropertyValidShouldNotAllowNullOrEmptyPropertyName()
        {
            var target = new MockValidatableObject();
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( null, string.Empty ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( "", string.Empty ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( null, string.Empty, new List<IValidationResult>() ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( "", string.Empty, new List<IValidationResult>() ) );
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Theory( DisplayName = "is property valid should return expected value" )]
        [InlineData( "", false )]
        [InlineData( "Test", true )]
        public void IsPropertyValidShouldReturnExpectedValue( string value, bool expected )
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Returns( expected );

            // act
            var actual = target.InvokeIsPropertyValid( "Name", value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "is property valid should return expected value with results" )]
        [InlineData( "", 1 )]
        [InlineData( "Test", 0 )]
        public void IsPropertyValidShouldReturnExpectedValueWithResults( string value, int expected )
        {
            // arrange
            var results = new List<IValidationResult>();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                         ( v, c, r ) =>
                         {
                             if ( c.MemberName == "Name" )
                             {
                                 if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                                 {
                                     r.Add( new Mock<IValidationResult>().Object );
                                 }

                                 return;
                             }

                             valid = true;
                         } )
                     .Returns( () => valid );

            // act
            target.InvokeIsPropertyValid( "Name", value, results );
            var actual = results.Count;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "validate property should not allow null or empty name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ValidatePropertyShouldNotAllowInvalidPropertyName( string propertyName )
        {
            // arrange
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeValidateProperty( propertyName, string.Empty ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "format error messages should return expected result" )]
        public void FormatErrorMessagesShouldReturnExpectedMessages()
        {
            // arrange
            var result1 = new Mock<IValidationResult>();
            var result2 = new Mock<IValidationResult>();

            result1.SetupGet( r => r.ErrorMessage ).Returns( string.Empty );
            result2.SetupGet( r => r.ErrorMessage ).Returns( "test" );

            var expected = new[] { result1.Object, result2.Object };
            var target = new MockValidatableObject();

            // act
            var actual = target.InvokeFormatErrorMessages( "Name", expected );

            Assert.Equal( 1, actual.Count() );
            Assert.Equal( "test", actual.First() );
        }

        [Theory( DisplayName = "set property should not allow null or empty name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void SetPropertyShouldNotAllowNullOrEmptyPropertyName( string propertyName )
        {
            // arrange
            var value = 0;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( propertyName, ref value, 1, EqualityComparer<int>.Default ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "set property should not allow null comparer" )]
        public void SetPropertyShouldNotAllowNullComparer()
        {
            // arrange
            IEqualityComparer<int> comparer = null;
            var value = 0;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( "Id", ref value, 1, comparer ) );

            // assert
            Assert.Equal( "comparer", ex.ParamName );
        }

        [Fact( DisplayName = "has errors should return expected value" )]
        public void HasErrorsShouldReturnExceptedValue()
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>( ( v, c, r ) => r.Add( new Mock<IValidationResult>().Object ) )
                     .Returns( false );

            // act
            target.Name = string.Empty;

            // assert
            Assert.True( target.HasErrors );
        }

        [Fact( DisplayName = "get errors should return errors" )]
        public void GetErrorsShouldSequenceOfErrors()
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject( validator.Object );
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                        ( v, c, r ) =>
                        {
                            switch ( c.MemberName )
                            {
                                case "Name":
                                case "Address":
                                    r.Add( new Mock<IValidationResult>().Object );
                                    valid = false;
                                    break;
                                default:
                                    valid = true;
                                    break;
                            }
                        } )
                     .Returns( () => valid );

            // act
            target.Name = string.Empty;
            target.Address = string.Empty;

            // assert
            Assert.Equal( 2, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 2, target.GetErrors().Count() );
        }
    }
}
