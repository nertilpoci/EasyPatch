## Easy Patch

A simple library to provide support for patching for you web api projects in a simple manner where you have control over what is going on.

### Asp.net Core

```markdown

//Install the package

Install-Package EasyPatch.AspnetCore



//Register it in Startup.cs
 public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options=> {
                options.UseEasyPatch();
            });
        }  

````



### Asp.net

```markdown

//Install the package
Install-Package EasyPatch.AspNetWebApi

// Configure it in your WebApi.Config
public static void Register(HttpConfiguration config)
        {

          ...............
           
           config.UseEasyPatch();
          .......
        }
```

```markdown
# Using the library in your api controllers

## The Entity

// Basic model, this will be the model you apply the patch to, could be your db entity when using entity framework
  public class User
    {
        public string Name { get; set; }
        public string Username { get; set; }
    }
    
    


## The patch object

The Patch Object should derive from EasyPatchModelBase wich has two generics. The Type of the patch object, and the object to patch the data into. In our case that would be User

 public class UserPatch : EasyPatchModelBase<UserPatch, User>
    {
        // UserPatchValidator: a validator that uses Fluent Validation to validate the incoming object.
        public UserPatch() : base(new UserPatchValidator())
        {
           // Set the properties you want to map the data into, you can map any property to any property aslong as the types allow for it
            AddPatchStateMapping(x => x.Name, x => x.Name);
            AddPatchStateMapping(x => x.Username, x => x.Username);
          
        }
        public String Name { get; set; }
        public String Username { get; set; }

        // Override this method, to get a list of validation errors. This must be overriden in order to get validation messages
        public override IEnumerable<KeyValuePair<string, string>> Validate()
        {
            return base.GetValidationErrors(this);
        }

    }


## The Validator

 public class UserPatchValidator : AbstractPatchValidator<UserPatch>
    {
        public UserPatchValidator()
        {
            //These rules apply allways proprty bound or not
            //RuleFor(x => x.Name).NotNull();
            //RuleFor(x => x.Username).NotNull();

            //This rules apply only when the property is bound
            WhenBound(x => x.Name, rule => rule.NotEmpty());
            WhenBound(x => x.Username, rule => rule.NotEmpty());
        }
    }

 If you need some properties to always be available when patching for example a **ModifyVersion** to keep track of changes so you don't override any then use the following rule
 RuleFor(x => x.Name).NotNull();
 This kind of rule will always be checked
 
 If you want properties to only be validated when they are bound then use this type of validation
 
 WhenBound(x => x.Name, rule => rule.NotEmpty());
 
 this rule will only run when a property is bound, meaning its on the payload that comes in
 
 
 
 
 
 # Api Controllers Aspnet Core
 
       [HttpPatch]
        public IActionResult Post([FromBody]UserPatch value)
        {
            // the modelstate will be populated with erors from the validator
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = _db.Users.FirstOrDefault();
            // assigns only the values that come in for patching
            value.Patch(user);
            return Ok(user);
        }
 
 
 # Api Controllers Aspnet
 
        [HttpPatch]
        public IHttpActionResult Post(TestPatchObject model)
        {
             // the modelstate will be populated with erors from the validator
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = _db.Users.FirstOrDefault();
            // assigns only the values that come in for patching
            model.Patch(user);
            return Ok(user);
        }
 
 
 

```

