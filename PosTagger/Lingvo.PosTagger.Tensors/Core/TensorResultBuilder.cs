﻿using System;

using M = System.Runtime.CompilerServices.MethodImplAttribute;
using O = System.Runtime.CompilerServices.MethodImplOptions;

namespace Lingvo.PosTagger.Tensors.Core
{
    /// <summary>
    /// 
    /// </summary>
    public static class TensorResultBuilder
    {
        // If a maybeResult is null, a new tensor will be constructed using the device id and element type of newTemplate
        public static Tensor GetWriteTarget( Tensor maybeResult, Tensor newTemplate, bool requireContiguous, params long[] requiredSizes )
            => GetWriteTarget( maybeResult, newTemplate.Allocator, newTemplate.ElementType, requireContiguous, requiredSizes );

        public static Tensor GetWriteTarget( Tensor maybeResult, IAllocator allocatorForNew, DType elementTypeForNew, bool requireContiguous, params long[] requiredSizes )
        {
            if ( maybeResult != null )
            {
                if ( !MatchesRequirements( maybeResult, requireContiguous, requiredSizes ) )
                {
                    throw (new InvalidOperationException( $"Output tensor does not match requirements. Tensor must have sizes {string.Join( ", ", requiredSizes )}{(requireContiguous ? "; and must be contiguous. " : ". ")}" +
                                                          $"Tensor's actual shape is '{string.Join( ", ", maybeResult.Sizes )}' and contiguous = '{maybeResult.IsContiguous()}'" ));
                }
                return (maybeResult);
            }
            else
            {
                return (new Tensor( allocatorForNew, elementTypeForNew, requiredSizes ));
            }
        }

        [M(O.AggressiveInlining)] private static bool MatchesRequirements( Tensor tensor, bool requireContiguous, params long[] requiredSizes )
        {
            if ( requireContiguous && !tensor.IsContiguous() )
            {
                return (false);
            }
            return ArrayEqual( tensor.Sizes, requiredSizes );
        }

        [M(O.AggressiveInlining)] public static bool ArrayEqual<T>( T[] a, T[] b )
        {
            if ( a.Length != b.Length )
            {
                return (false);
            }

            for ( int i = 0; i < a.Length; ++i )
            {
                if ( !a[ i ].Equals( b[ i ] ) )
                {
                    return (false);
                }
            }
            return (true);
        }

        [M(O.AggressiveInlining)] public static bool ArrayEqualExcept<T>( T[] a, T[] b, int ignoreIndex )
        {
            if ( a.Length != b.Length )
            {
                return (false);
            }

            for ( int i = 0; i < a.Length; ++i )
            {
                if ( i == ignoreIndex )
                {
                    continue;
                }

                if ( !a[ i ].Equals( b[ i ] ) )
                {
                    return (false);
                }
            }
            return (true);
        }
    }
}
