using EasyPatch.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EasyPatch.Common.Implementation
{
    public abstract class EasyPatchModelBase<TRequest, TModel> : IEasyPatchModel<TRequest, TModel>
   where TRequest : class, IEasyPatchModel<TRequest, TModel>, new()
    {

        public EasyPatchModelBase() { }
        public EasyPatchModelBase(AbstractPatchValidator<TRequest> validator)
        {
            _validator = validator;
        }

        protected AbstractPatchValidator<TRequest> _validator;
        protected readonly IList<string> boundProperties = new List<string>();
        private BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
        protected readonly IDictionary<string, Action<TModel>> patchStateMapping = new Dictionary<string, Action<TModel>>(StringComparer.InvariantCultureIgnoreCase);

        public void AddBoundProperty(string propertyName)
        {
            if (!boundProperties.Contains(propertyName, StringComparer.InvariantCultureIgnoreCase)) boundProperties.Add(propertyName);     
        }

        public TRequest AddPatchStateMapping<TProperty, TModelProperty>(
            Expression<Func<TRequest, TProperty>> propertyExpression,
            Expression<Func<TModel, TModelProperty>> modelMapping)
        {
            var propertyName = GetPropertyName(propertyExpression);

            var instanceProperty = GetType().GetProperty(propertyName, bindingFlags);

            Action<TModel> mappingAction = (model) =>  BuildActionFromExpression(modelMapping)(model, (TModelProperty)instanceProperty.GetValue(this, null));

            AddPatchStateMapping(propertyExpression, mappingAction);

            return this as TRequest;
        }

        public TRequest AddPatchStateMapping<TProperty>(
            Expression<Func<TRequest, TProperty>> propertyExpression,
            Action<TModel> propertyToModelMapping = null)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (propertyToModelMapping == null)
            {
                propertyToModelMapping = (model) =>
                {
                    var modelProperty = model.GetType().GetProperty(propertyName, bindingFlags);

                    var instanceProperty = GetType().GetProperty(propertyName, bindingFlags);

                    if (modelProperty != null && instanceProperty != null)
                    {
                        modelProperty.SetValue(model, instanceProperty.GetValue(this, null), null);
                    }
                };
            }

            if (patchStateMapping.ContainsKey(propertyName))
            {
                patchStateMapping[propertyName] = propertyToModelMapping;
            }
            else
            {
                patchStateMapping.Add(propertyName, propertyToModelMapping);
            }

            return this as TRequest;
        }

        private Action<TObject, TProperty> BuildActionFromExpression<TObject, TProperty>(
            Expression<Func<TObject, TProperty>> accessor)
        {
            if (accessor == null)
                throw new ArgumentNullException(nameof(accessor));

            var memberExpression = accessor.Body as MemberExpression;

            var memberInfo = memberExpression?.Member;
            if (!(memberInfo is PropertyInfo) && !(memberInfo is FieldInfo))
                throw new InvalidOperationException("Member is not a property or field");

            var valueParameter = Expression.Parameter(typeof(TProperty), "val");
            var assignmentExpression = Expression.Assign(memberExpression, valueParameter);
            var lambdaExpression =
                Expression.Lambda<Action<TObject, TProperty>>(
                    assignmentExpression,
                    accessor.Parameters[0],
                    valueParameter);

            return lambdaExpression.Compile();
        }

        private string GetPropertyName<TProperty>(Expression<Func<TRequest, TProperty>> propertyExpression)
        {
            var propertyBody = propertyExpression.Body as MemberExpression;

            if (propertyBody == null)
            {
                throw new InvalidCastException($"Cannot get property name from {nameof(propertyExpression)}.");
            }
            else
            {
                var fullPropertyName = propertyBody.ToString();

                return fullPropertyName.Substring(fullPropertyName.IndexOf('.') + 1);
            }
        }

        public bool IsBound<TProperty>(Expression<Func<TRequest, TProperty>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);

            return boundProperties.Contains(propertyName, StringComparer.InvariantCultureIgnoreCase);
        }

        public void Patch(TModel model)
        {
            foreach (var kvp in patchStateMapping)
            {
                if (boundProperties.Contains(kvp.Key, StringComparer.InvariantCultureIgnoreCase))
                {
                    kvp.Value(model);
                }
            }
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> GetValidationErrors(TRequest request)
        {
            return _validator != null ? _validator.Validate(request).Errors.Select(z => new KeyValuePair<string, string>(z.PropertyName, z.ErrorMessage)) : Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public abstract IEnumerable<KeyValuePair<string, string>> Validate();
    }

}
