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

## Pages

Files in the pages directory are rendered in the output root directory directly so they can be reached on the `/pagename` path. SReserved therefore are not allowed for pages:

* assets
* pages
* posts
* tags
* index

Page metadata is passed in the file's front matter header and content is parsed as markdown. Currently pages only have `title` metadata:

```
---
# Front matter header is between --- marks
title: 'About'
---

# Here comes the markdown content

...
```

## Posts

Posts are generated based on their filename and available on `/posts/post-file-name` path so it is recommended to name the original file seo-friendly. The format is the same front matter + markdown as with pages. Example:

```
---
# Front matter header is between --- marks

# Title of the post
title: 'My first blog post'

# Excerpt that can be used on list page for example
excerpt: 'A longer description of my post'

# Date of creation (list page orders by this)
createdAt: '2020-04-01'

# Author
postedBy: 'codernr'

# Tags
tags: [ 'blog', 'C#', 'Github' ]
---

# Here comes the markdown content

...
```

## Tags

Tags are collected from posts' metadata and **grouped by slug**. Special characters are stripped and hyphens are added when generating slugs so `C#` and `C` becomes the same, this should be kept in mind when tagging posts. Tag pages are generated under `/tag/tagname` path where a list of posts using that tag is passed to the template.

## Post pagination

Posts are ordered descending by creation date and paginated as defined in config. The first page is always rendered as `index.html` and the other pages are rendered under `/pages/{page-number}` path.

## Assets

There are two sources of assets:

* site-wide assets in `source/assets` directory
* template specific assets in `source/templates/<selected-template>/assets`

These folders are merged during generation to the `/assets` path which means that a template asset with the same name as a site asset overwrites it.

## Templates

Bloggen.Net uses [Handlebars.Net](https://github.com/rexm/Handlebars.Net) as a templating engine. The pages are rendered from a main template file `index.hbs` and embedded layout files that are specific for the type of content.

There is a site variable that is available within all the templates called site with the following structure:

```js
{
  config: {
    title: '...',
    subheading: '...',
    template: '...',
    postsperpage: '...'
    // ... and al the config values
  },
  tags: [ // all the tags ordered by name
    { /* see details at tag layout description */ }
  ],
  pages: [
    { /* see details at list layout description */ }
  ]
}
```

### List layout (layouts/list.hbs)

This template is rendered with the paginated post objects. Pagination objects are available in `{{site.pages}}` array in the template. Structure:

```js
{
  pageNumber: 2,
  items: [ { /* see structure at post layout */ }]
  url: '/pages/2' // or '/' if on the first page that is index.html
  totalcount: 3 // number of pages
  previous: { /* the previous pagination object */ }
  next: { /* the next pagination object */ }
}
```

### Page layout (layouts/page.hbs)

Pages are rendered with this template. Page data is available in the {{data}} variable in the template. Structure is the same as described in [posts](#pages) section.

Page content is available in the {{content}} variable. To render markdown as html see [helpers](#template-helpers).

### Post layout (layouts/post.hbs)

The same as pages. Metadata is in the {{data}} variable, content is in {{content}}. See metadata in [posts](#posts) section.

### Tag layout (layouts/tag.hbs)

This template is used to render one specific tag's post list. Tag metadata is available in {{data}} variable:

```js
{
  name: 'tag name',
  postreferences: [ { /* see details at posts description */ } ]
  url: '/tags/tag-name'
}
```

### Template helpers

There are two registered helpers in Bloggen.Net by default:

* Date helper that renders date objects with the format string from config; usage: `{{date datevariable}}
* Html helper that renders markdown content as html; usage: `{{html content}}`