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

## Your binding model
## Header 2
### Header 3

- Bulleted
- List

1. Numbered
2. List

**Bold** and _Italic_ and `Code` text

[Link](url) and ![Image](src)
```

For more details see [GitHub Flavored Markdown](https://guides.github.com/features/mastering-markdown/).

### Jekyll Themes

Your Pages site will use the layout and styles from the Jekyll theme you have selected in your [repository settings](https://github.com/nertilpoci/EasyPatch/settings). The name of this theme is saved in the Jekyll `_config.yml` configuration file.

### Support or Contact

Having trouble with Pages? Check out our [documentation](https://help.github.com/categories/github-pages-basics/) or [contact support](https://github.com/contact) and we’ll help you sort it out.
