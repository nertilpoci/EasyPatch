using EasyPatch.Common.Implementation;
using EasyPatch.CoreApi.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPatch.CoreApi.Validators
{

    public class TestObjectPatchValidator : AbstractPatchValidator<TestPatchObject>
    {
        public TestObjectPatchValidator()
        {
            //These rules apply allways proprty bound or not
            //RuleFor(x => x.Property1).NotNull();
            //RuleFor(x => x.Property2).NotNull();

            //This rules apply only when the property is bound
            WhenBound(x => x.Property1, rule => rule.NotEmpty());
            WhenBound(x => x.Property2, rule => rule.NotEmpty());
        }
    }
}