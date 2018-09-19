using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EasyPatch.Common.Interface
{
    public interface IEasyPatchModel<TRequest, TModel> : IEasyPatchModel<TRequest>
    where TRequest : class, IEasyPatchModel<TRequest, TModel>, new()
    {
        TRequest AddPatchStateMapping<TProperty>(
            Expression<Func<TRequest, TProperty>> propertyExpression,
            Action<TModel> propertyToModelMapping = null);
        void Patch(TModel model);
    }

    public interface IEasyPatchModel<TRequest> : IEasyPatchModel
    {
        bool IsBound<TProperty>(Expression<Func<TRequest, TProperty>> propertyExpression);
    }

    public interface IEasyPatchModel
    {
        void AddBoundProperty(string propertyName);
        IEnumerable<KeyValuePair<string, string>> Validate();
    }
}
