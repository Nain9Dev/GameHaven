# Specification: Installable site (Android and iOS)

Status: Implemented on `feat/pwa-install-gamehaven`, not merged

## Objective

A visitor on a phone can install `gamehaven.naindev.com` from the browser and open it like an app: its own icon
and name on the home screen and a window with no browser bar. Android Chrome and iOS Safari are the targets.

The shared contract and the verified browser criteria live in the portfolio spec
(`nain9dev.github.io`, `specs/next-pwa-install/spec.md`). This repository follows it unchanged.

## Requirements (EARS)

Consolidated in `docs/10-requirements.md` as REQ-019 to REQ-021.

- **REQ-019**: When a phone browser loads the client entry page, the page shall link `manifest.webmanifest`,
  which declares `id`, `start_url` and `scope` as `./`, `display` as `standalone`, `name` and `short_name` as
  `GameHaven`, the page's meta description, `lang` as `en`, `dir` as `ltr`, and `theme_color` and
  `background_color` as `#090a0f` (the first paint, `--bg-deep`). It shall not declare
  `prefer_related_applications`.
- **REQ-020**: When the manifest is read, it shall declare three PNG icons that exist at the declared size:
  192 px and 512 px with purpose `any`, and 512 px with purpose `maskable`; a 180 px `apple-touch-icon` shall
  exist beside them in `icons/`.
- **REQ-021**: When the client entry page is opened on iOS Safari, the page shall carry `apple-touch-icon`,
  `apple-mobile-web-app-title` equal to `GameHaven`, `mobile-web-app-capable` and
  `apple-mobile-web-app-capable` set to `yes`, `apple-mobile-web-app-status-bar-style` set to `black` (dark
  site) and a `theme-color` meta.

## Edge cases and error handling

- The page loads no service worker and the build adds none: an installed window without network shows the
  browser's own offline screen. Installable only, by owner decision of 2026-11-13.
- The static host serves the files from `dist/` as published; `404.html` is a copy of `index.html`, so it carries
  the same tags.
- The page declared `/favicon.svg` but the repository had no such file (the live URL answered 404). The mark
  of the navbar (gamepad glyph on the indigo tile) is now committed as `public/favicon.svg` and is the source of
  the install icons.
- No Content-Security-Policy is set, so a same-origin manifest is not blocked.
- `GameHaven.WebUI` (Blazor, not published) is out of scope.

## Out of scope

- A service worker, offline pages or any cache; an install button; screenshots for the Android install dialog.
- Opacity and safe-zone checks of the PNG files: the icon generator and the shared `verify-install` script
  cover them; the unit test reads only the PNG header.

## Acceptance criteria

- `dotnet test GameHaven.slnx` passes with `PwaInstallTests`.
- Chrome reports no installability error on the built `GameHaven.Client/dist` (DevTools protocol
  `Page.getInstallabilityErrors`).
- After the deploy, the live manifest answers 200 and the owner installs the site on an Android phone and an
  iPhone: the icon shows the mark uncropped, the label reads `GameHaven` and the window has no browser bar.

## Session state

- Done: manifest, icons, head tags, test, docs. Verified locally: unit tests, `npm run build`, installability.
- Next: T05 after the merge (live check and phone install).
- Blocked: nothing. CI (`deploy.yml`) does not run `dotnet test`; the test runs locally only.
