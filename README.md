<div align=center>
 <img alt="Pong Logo" src="assets/logo.png" width="50%">
 <br>
 <img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/release/WilliamRagstad/Pond">
 <img alt="GitHub All Releases" src="https://img.shields.io/github/downloads/WilliamRagstad/Pond/total">
 <img alt="GitHub contributors" src="https://img.shields.io/github/contributors/WilliamRagstad/Pond">
 <img alt="GitHub Release Date" src="https://img.shields.io/github/release-date/WilliamRagstad/Pond?label=latest%20release">
</div>





# Welcome

Have you've ever wanted to make a website from your *Markdown* just with the click of a button? Then I welcome you to Pond, a fully customizable and flexible static website generator modelled after [Django](https://docs.djangoproject.com/en/3.0/ref/templates/language/)’s *(or [Jinja](https://jinja.palletsprojects.com/en/2.11.x/)'s)* templates.

Pond let's you write everything in Markdown and instantly produce a complete website of your liking ready for publication.

# Usage

```bash
Pond Command Line v1.0.0

Usage: Pond (options)

Options:
¨¨¨¨¨¨¨¨
  /c (file)  Configuration file
  
```



## Configuration

As promised, Pond comes with with extensive customization and flexibility so you can have as much freedom as possible when designing your templates.

You write all the content in Markdown, which is then combined with the correct HTML template and CSS stylesheet to produce a finished page.

[This is not yet written]



## Example page

Markdown article:

```markdown
# My fancy title
## Welcome
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent tincidunt metus ac velit facilisis elementum. Nam iaculis sagittis ante, non rhoncus risus mattis at. Integer semper felis ac pretium placerat. Maecenas placerat ipsum ut odio egestas, rutrum gravida velit suscipit. Sed ornare, sapien non ornare ultrices, mi mauris ullamcorper magna, eget sollicitudin nibh tellus ac nunc. Curabitur tempus ornare ornare. Pellentesque urna diam, bibendum nec urna tristique, malesuada iaculis odio. Integer finibus magna ac nunc scelerisque commodo. Nunc porta massa quis est porttitor lobortis. Quisque finibus lorem vitae ante consequat tincidunt. Aliquam efficitur eros vitae ligula tempor, a interdum ex aliquet. Mauris nibh nibh, mollis vitae est quis, tempus sollicitudin mauris. Vestibulum cursus, ligula sit amet sodales venenatis, orci sem sagittis est, eu mattis orci tellus a nibh. Vestibulum tempus lacus sed nunc imperdiet porttitor. Morbi gravida ullamcorper fermentum.

![Maybe an image](assets/img.png)

## The end
Praesent cursus elit id lacus hendrerit, ac interdum ante euismod. Interdum et malesuada fames ac ante ipsum primis in faucibus. Sed sollicitudin suscipit hendrerit. Pellentesque faucibus eget eros at auctor. Ut pulvinar ante vel neque dignissim, gravida pulvinar felis pellentesque. Ut fermentum molestie efficitur. Praesent molestie vehicula enim vitae tincidunt. Quisque vel pellentesque dui. Morbi a arcu nulla. Nullam aliquet vel quam et vehicula. Aliquam erat volutpat.
```

HTML Template:

```html
<html>
    <head>
        <title>Here is {{ title }}</title>
    </head>
    <body>
        
    </body>
</html>
```

