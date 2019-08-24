﻿// ------------------------------------------------------------------------------
// <copyright file="NewExpressionRenderer.cs" company="Drake53">
// Copyright (c) 2019 Drake53. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ------------------------------------------------------------------------------

#pragma warning disable SA1649 // File name should match first type name

using War3Net.CodeAnalysis.Jass.Syntax;

namespace War3Net.CodeAnalysis.Jass.Renderer
{
    public partial class JassRenderer
    {
        public void Render(NewExpressionSyntax newExpression)
        {
            Render(newExpression.Expression);
            if (newExpression.EmptyExpressionTail is null)
            {
                Render(newExpression.ExpressionTail);
            }
        }
    }
}