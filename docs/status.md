# Project status and intended use

Pond is an experimental Markdown-to-static-site generator. The maintained example
in this checkout demonstrates a focused workflow: multiple Markdown files become
corresponding HTML pages, with layout controlled by reusable templates containing
C# expressions, loops, and conditions. The build and integration checks establish
that this fixture works; they do not establish complete Markdown or template
language support.

Use Pond when you want to keep ordinary Markdown content separate from a small
site's HTML layout, particularly when maintaining existing Pond templates. Start
with the checked-in fixture and the [configuration reference](configuration.md).
The example preserves Pond's existing template model rather than introducing a
new rendering engine.

The renderer handles the example's paragraphs, headings, links, emphasis, lists,
and code. Other Markdown constructs can fall back to their string representation;
full CommonMark conformance is not claimed. There is no automatic navigation,
Markdown link rewriting, heading-anchor generation, general asset pipeline,
preview server, watch mode, or PDF export. Templates and content are trusted
build inputs. The .NET 8 source build replaces the old .NET Core 3.1 build target;
older release binaries may behave differently.

## Relationship to typeset_md

[typeset_md](https://github.com/WilliamRagstad/typeset_md) describes a different
approach: a Rust compiler for extended Markdown with embedded Starlark, mathematical
content, and document-oriented presentation. Its README also describes themes,
HTML/PDF output goals, and a roadmap; several features, including PDF generation
and theme specification files, are marked planned or TODO. Consult its current
implementation before relying on those features. This repair does not validate
typeset_md's capabilities.

| Need | Intended fit |
| --- | --- |
| Separate ordinary Markdown articles from reusable HTML page layouts; preserve Pond's C# template syntax | Pond's fixture workflow |
| Explore computation embedded in Markdown, mathematical documents, or the Rust/Starlark approach to typesetting | Evaluate typeset_md against its current implementation and roadmap |

The tools do not share a configuration schema or template language. Pond's
`{{ C# expression }}` tags live in HTML templates; typeset_md's documented Starlark
syntax lives in extended Markdown. This work neither declares Pond deprecated nor
promises a migration path, compatibility layer, or replacement relationship.
