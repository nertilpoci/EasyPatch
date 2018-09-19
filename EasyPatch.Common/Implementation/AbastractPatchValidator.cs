using EasyPatch.Common.Interface;
using FluentValidation;
using System;
using System.Linq.Expressions;

namespace EasyPatch.Common.Implementation
{
    public abstract class AbstractPatchValidator<T> : AbstractValidator<T>
    where T : IEasyPatchModel<T>
    {
        public void WhenBound<TProperty>(
            Expression<Func<T, TProperty>> propertyExpression,
            Action<IRuleBuilderInitial<T, TProperty>> action)
        {
            When(x => x.IsBound(propertyExpression), () => action(RuleFor(propertyExpression)));
        }
    }
}
