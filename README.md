# Bloggen.Net ![.NET Core](https://github.com/codernr/bloggen-net/workflows/.NET%20Core/badge.svg) ![Codacy Badge](https://api.codacy.com/project/badge/Grade/5d3fca2276ea4c399f5b7e5d7cb3649f)
Dotnet core cross platform static blog generator based on markdown, YAML front matter and Handlebars.NET

## Features

* Uses YAML front matter for metadata and markdown
* Generates posts and static pages
* Handles tags
* Supports pagination of posts
* Uses [Handlebars](https://handlebarsjs.com/guide/) language for templates

## Usage

1. Download the [latest release](https://github.com/codernr/bloggen-net/releases/latest)
2. Extract the tar file
3. Go to the extracted directory
4. Run `dotnet ./Bloggen.Net.dll -s <source directory path> -o <output directory path>`

## Source directory structure

The source directory has to follow a well defined structure (see details in next sections):

```
source/
├── assets/
│   ├── img/
│   │   ├── some_image.jpg
│   │   └── ...
│   ├── js/
│   └── ...
├── pages
│   ├── some-page.md
│   ├── other-page.md
│   └── ...
├── posts
│   ├── my-first-post.md
│   ├── my-second-post.md
│   └── ...
├── templates
│   ├── my-template-name/
│   │   ├── assets/
│   │   │   ├── img/
│   │   │   │   ├── template_img.jpg
│   │   │   │   └── ...
│   │   │   ├── css/
│   │   │   │   ├── template_style.css
│   │   │   │   └── ...
│   │   │   ├── js/
│   │   │   │   ├── template_scripts.js
│   │   │   │   └── ...
│   │   │   └── ...
│   │   ├── layouts/
│   │   │   ├── list.hbs
│   │   │   ├── page.hbs
│   │   │   ├── post.hbs
│   │   │   └── tag.hbs
│   │   ├── partials/
│   │   │   ├── any_partial.hbs
│   │   │   └── ...
│   │   └── index.hbs
│   └── ...
└── config.yml
```

## Config

The source root directory contains a `config.yml` file that represents the site configuration. Example:

```yml
# The title of the site; default: empty string
title: 'Site title'

# A subheading/slogan; default: empty string
subHeading: 'A secondary heading'

# source/templates directory can contain multiple templates,
# the one with this name is rendered; default: empty string
template: 'my-template-name'

# Number of posts that is displayed on one page; default: 10
postsPerPage: 10

# Root url of site; default: '/'
url: 'https://my-page.com'

# Date format string on the site; default: 'yyyy-MM-dd'
dateFormat: 'yyyy-MM-dd'

# Site-wide meta properties, e.g. 'og:title', etc.
metaProperties:
  - property: 'og:image'
    content: '/assets/my-facebook-share-image.jpg'
```