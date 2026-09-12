# Summaries Brand System

## Scope

This backend has no web client or shared frontend stylesheet. The brand system therefore has two jobs:

1. provide a stable visual contract for the future client application;
2. keep transactional email recognizable, accessible, and consistent today.

The current email implementation applies these tokens through `EmailTheme` so individual templates do not drift when a palette or typography decision changes.

## Design Direction

Summaries should feel like a well-kept reading desk: warm paper, deep ink, and one confident signal color for action. The visual language is editorial but operational. It should make books and reading progress feel considered without turning routine account and payment messages into decoration.

## Core Tokens

| Token | Value | Use |
| --- | --- | --- |
| Canvas | `#f4efe6` | Page or email background. |
| Surface | `#fffdf8` | Primary content surface. |
| Ink | `#172032` | Headings, primary labels, important values. |
| Muted | `#5f6b7a` | Body copy and secondary metadata. |
| Quiet | `#8b96a3` | Supporting notes and footer copy. |
| Faint | `#b8c0c8` | Timestamps and low-emphasis metadata. |
| Border | `#e2dcd1` | Dividers and quiet boundaries. |
| Accent | `#e85d3f` | Primary actions, links, and attention states. |

Use the accent sparingly. Do not use it as a full-page background or for large blocks of body text. Pair it with Ink or white text only after checking contrast in the final component.

## Typography

Use a distinctive but dependable type pairing:

- display and headings: a readable editorial serif such as `Georgia` or the client application's licensed brand serif;
- interface and body copy: `Trebuchet MS`, `Verdana`, or the client application's approved sans-serif;
- codes and technical values: a monospace face with generous character spacing.

Email clients have uneven font support. The email templates use a safe sans-serif stack and inline styles. A future web client may use hosted fonts, but every critical label must remain legible when the font fails to load.

## Layout Rules

- Prefer generous whitespace and strong alignment over decorative containers.
- Keep content surfaces compact and readable, with a shared maximum width of 560px for transactional email and future application pages.
- Use one content container token across pages. Do not create per-page widths unless the content type genuinely requires a wider data surface.
- Use a 4px radius for brand surfaces and buttons. Avoid pill-shaped controls unless the control represents a status or tag.
- Use one primary action per message. Secondary links should look secondary.
- Keep body copy between 14px and 16px on web surfaces; transactional email may use 14px with at least 22px line height.
- Never put essential information in an image, including a logo-only action or payment instruction.
- Use real text alternatives for logos and meaningful imagery.

## Email Requirements

Transactional email must remain usable in narrow clients and text-forward previews:

- use table-based layout and inline styles;
- include a meaningful `<title>` and `lang="en"`;
- provide a visible fallback URL for every important action;
- keep links descriptive and distinguishable without color alone;
- preserve a clear expiry or security note where relevant;
- never make the entire message an invisible link;
- keep provider, token, and recipient data out of diagnostic output.

The shared email theme is implemented in `src/Summaries.Infrastructure/Email/EmailTheme.cs`. New templates should use `EmailTheme.Apply` and the existing structural conventions rather than introducing new color literals.

## Dark Theme

The dark theme is the default alternate mode for future client pages and is opt-in automatically in email clients through `prefers-color-scheme: dark`.

| Token | Dark value | Use |
| --- | --- | --- |
| Canvas | `#0d1117` | Full-page background. |
| Surface | `#151b24` | Content panels and reading surfaces. |
| Ink | `#f4f7fb` | Headings and primary content. |
| Muted | `#b8c2cf` | Body copy and secondary content. |
| Border | `#2c3745` | Dividers and boundaries. |
| Accent | `#ff8065` | Actions, links, and attention states. |

Dark mode must preserve the same width, spacing, hierarchy, focus treatment, and interaction semantics as light mode. Switch colors through tokens rather than duplicating page-specific styles. Respect the user's system preference by default, and provide an explicit client-level theme setting when the frontend exists.

## Future Frontend Contract

When the client application is added to this workspace, it should expose the same tokens as CSS custom properties or the client framework's native token mechanism:

```css
:root {
  --color-canvas: #f4efe6;
  --color-surface: #fffdf8;
  --color-ink: #172032;
  --color-muted: #5f6b7a;
  --color-accent: #e85d3f;
  --color-border: #e2dcd1;
  --radius-control: 4px;
  --content-width: 560px;
}

[data-theme="dark"] {
  --color-canvas: #0d1117;
  --color-surface: #151b24;
  --color-ink: #f4f7fb;
  --color-muted: #b8c2cf;
  --color-accent: #ff8065;
  --color-border: #2c3745;
}
```

The client must also preserve the API's callback origin, email links, authentication states, and purchase/download states. A visual redesign must not change endpoint semantics or silently alter token handling.

## Accessibility Review

Every new user-facing surface should be checked for keyboard access, visible focus, logical heading order, minimum readable text size, color contrast, reduced-motion behavior, and mobile layout. Do not encode success, warning, or failure using color alone; pair color with text, an icon, or a structural change.