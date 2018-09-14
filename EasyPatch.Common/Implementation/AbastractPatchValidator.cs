using EasyPatch.Common.Interface;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EasyPatch.Common.Implementation
{
    public abstract class AbstractPatchValidator<T> : AbstractValidator<T>
    where T : IPatchState<T>
    {
        public void WhenBound<TProperty>(
            Expression<Func<T, TProperty>> propertyExpression,
            Action<IRuleBuilderInitial<T, TProperty>> action)
        {
            When(x => x.IsBound(propertyExpression), () => action(RuleFor(propertyExpression)));
        }
    }
}
