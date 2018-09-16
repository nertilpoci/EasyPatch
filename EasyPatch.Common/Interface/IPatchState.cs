using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EasyPatch.Common.Interface
{
    public interface IPatchState<TRequest, TModel> : IPatchState<TRequest>
    where TRequest : class, IPatchState<TRequest, TModel>, new()
    {
        TRequest AddPatchStateMapping<TProperty>(
            Expression<Func<TRequest, TProperty>> propertyExpression,
            Action<TModel> propertyToModelMapping = null);
        void Patch(TModel model);
    }

    public interface IPatchState<TRequest> : IPatchState
    {
        bool IsBound<TProperty>(Expression<Func<TRequest, TProperty>> propertyExpression);
    }

    public interface IPatchState
    {
        void AddBoundProperty(string propertyName);
        IEnumerable<KeyValuePair<string, string>> Validate();
    }
}
