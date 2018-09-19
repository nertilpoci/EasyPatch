using EasyPatch.Common.Implementation;
using EasyPatch.CoreApi.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyPatch.CoreApi.Models
{

    public class UserPatch : EasyPatchModelBase<UserPatch, User>
    {
        public UserPatch() : base(new UserPatchValidator())
        {
            AddPatchStateMapping(x => x.Name, x => x.Name);
            AddPatchStateMapping(x => x.Username, x => x.Username);
          
        }
        public String Name { get; set; }
        public String Username { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> Validate()
        {
            return base.GetValidationErrors(this);
        }

    }
}