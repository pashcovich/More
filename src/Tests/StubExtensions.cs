﻿namespace Microsoft.QualityTools.Testing.Fakes.Stubs
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Provides extension methods for verifying stubs generated by the Microsoft Fakes framework.
    /// </summary>
    public static class StubExtensions
    {
        private static object GetLiteral( this Expression expression )
        {
            var lambda = Expression.Lambda( typeof( Func<object> ), Expression.Convert( expression, typeof( object ) ) );
            var eval = (Func<object>) lambda.Compile();
            return eval();
        }

        private static void VerifyMethod( this StubObserver observer, Type declaringType, MethodCallExpression call, int times )
        {
            var calls = observer.GetCalls().Where( c => c.StubbedType.Equals( declaringType ) && c.StubbedMethod.Equals( call.Method ) ).ToArray();
            var userMessage = $"The method {call.Method.Name} was expected to be called {times} time(s), but was actually called {calls.Length} time(s).";

            Assert.True( calls.Length == times, userMessage );

            // there are no parameters to verify (e.g. no calls or multiple calls; thus the expression parameters can't be verified)
            if ( times != 1 )
                return;

            var parameters = new Lazy<ParameterInfo[]>( call.Method.GetParameters );
            var args = calls[0].GetArguments();

            for ( var i = 0; i < call.Arguments.Count; i++ )
            {
                var actual = args[i];

                if ( actual == null )
                {
                    // if the parameter is an out parameter, we cannot validate the value at return.
                    // IStubObserver only supports entering a method, not existing.
                    if ( parameters.Value[i].IsOut )
                        continue;
                }

                var expected = call.Arguments[i].GetLiteral();
                Assert.True( object.Equals( expected, actual ), userMessage );
            }
        }

        /// <summary>
        /// Verifies the specified expression was observed.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of stub to verify.</typeparam>
        /// <typeparam name="TResult">The result <see cref="Type">type</see> of the stubbed method.</typeparam>
        /// <param name="observer">The <see cref="IStubObserver">observer</see> to verify.</param>
        /// <param name="expression">The <see cref="Expression{T}">expression</see> representing the stubbed method to verify.</param>
        /// <param name="times">The number of times the method is expected to have been called.</param>
        public static void Verify<T, TResult>( this IStubObserver observer, Expression<Func<T, TResult>> expression, int times ) where T : class
        {
            Contract.Requires( observer != null );
            Contract.Requires( times >= 0 );

            var source = observer as StubObserver;

            if ( source == null )
                throw new Exception( $"The type {observer.GetType()} was unexpected. {typeof( StubObserver )} is assumed and the only {typeof( IStubObserver )} supported." );

            // TODO: add support for properties when needed

            var method = (MethodCallExpression) expression.Body;
            source.VerifyMethod( typeof( T ), method, times );
        }

        /// <summary>
        /// Verifies the specified expression was invoked against the stub.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type">type</see> of stub to verify.</typeparam>
        /// <typeparam name="TResult">The result <see cref="Type">type</see> of the stubbed method.</typeparam>
        /// <param name="stub">The <see cref="StubBase{T}">stub</see> to verify.</param>
        /// <param name="expression">The <see cref="Expression{T}">expression</see> representing the stubbed method to verify.</param>
        /// <param name="times">The number of times the method is expected to have been called.</param>
        public static void Verify<T, TResult>( this StubBase<T> stub, Expression<Func<T, TResult>> expression, int times ) where T : class
        {
            Contract.Requires( stub != null );
            Contract.Requires( times >= 0 );

            var observer = stub.InstanceObserver;

            if ( observer == null )
                throw new Exception( $"An {typeof( IStubObserver )} has not been setup." );

            stub.InstanceObserver.Verify( expression, times );
        }
    }
}
