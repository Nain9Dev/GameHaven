# Plan: Installable site

## Published folder

`GameHaven.Client` is a Vite project (`base: './'`). `deploy.yml` runs `npm ci` and `npm run build` there, copies
`dist/index.html` to `dist/404.html` and uploads `GameHaven.Client/dist`. Vite copies `public/` to the root of
`dist/` unchanged, so no workflow change is needed. The build rewrites the head links to `./manifest.webmanifest`
and `./icons/apple-touch-icon.png`; the manifest itself uses `./` paths, matching `base: './'`.

## Files

| File | Change |
| :--- | :--- |
| `GameHaven.Client/public/manifest.webmanifest` | new |
| `GameHaven.Client/public/icons/` | new: `icon-192.png`, `icon-512.png`, `icon-maskable-512.png`, `apple-touch-icon.png` |
| `GameHaven.Client/public/favicon.svg` | new: the navbar mark, previously a dangling link |
| `GameHaven.Client/index.html` | manifest link, iOS tags, `theme-color` |
| `GameHaven.UnitTests/Seo/PwaInstallTests.cs` | new test (xUnit, reads the client files from the repository tree) |
| `docs/10-requirements.md`, `docs/50-traceability.md` | REQ-019 to REQ-021 |

## Decisions

- Colours: the page paints `--bg-deep` (`#090a0f`) first, a dark site, so the status bar style is `black`.
- Test home: the client has no test runner, and the only suite is `GameHaven.UnitTests`, which already reads the
  client files for the SEO baseline. `deploy.yml` does not run it; that gap predates this change.
- No service worker, cache or install button.

## Icon command

Run from the repository root (`--bg` is the middle of the tile gradient, so the maskable icon blends in):

```
node make-install-icons.mjs --source GameHaven.Client/public/favicon.svg --out GameHaven.Client/public/icons --bg "#5a52e9" --maskable-scale 0.9
```

## Verification

- `dotnet test GameHaven.slnx`: red before the change (3 failures), green after.
- `npm ci && npm run build` in `GameHaven.Client`.
- `verify-install.mjs` on `GameHaven.Client/dist` (with `index.html` and `404.html`); `installability.mjs` on the
  same folder.
