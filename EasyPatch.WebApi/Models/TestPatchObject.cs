using EasyPatch.Common.Implementation;
using EasyPatch.WebApi.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace EasyPatch.WebApi.Models
{

    public class TestPatchObject : EasyPatchModelBase<TestPatchObject, TestEntity>
    {
        public TestPatchObject() : base(new TestObjectPatchValidator())
        {
            AddPatchStateMapping(x => x.Property1, x => x.Property1);
            AddPatchStateMapping(x => x.Property2, x => x.Property2);
          
        }
        public String Property1 { get; set; }
        public String Property2 { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> Validate()
        {
            return base.GetValidationErrors(this);
        }

    }
}