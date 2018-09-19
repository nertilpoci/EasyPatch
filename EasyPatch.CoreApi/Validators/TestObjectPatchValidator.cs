using EasyPatch.Common.Implementation;
using EasyPatch.CoreApi.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPatch.CoreApi.Validators
{

    public class UserPatchValidator : AbstractPatchValidator<UserPatch>
    {
        public UserPatchValidator()
        {
            //These rules apply allways proprty bound or not
            //RuleFor(x => x.Property1).NotNull();
            //RuleFor(x => x.Property2).NotNull();

            //This rules apply only when the property is bound
            WhenBound(x => x.Name, rule => rule.NotEmpty());
            WhenBound(x => x.Username, rule => rule.NotEmpty());
        }
    }
}