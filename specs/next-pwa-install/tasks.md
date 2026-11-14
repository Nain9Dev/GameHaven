# Tasks: Installable site

Labels: `[A]` agent alone, `[M]` mixed, `[H]` owner only.

- [x] **T01 [A]** Write `PwaInstallTests.cs` first. Done when it fails on the unchanged tree (3 failures).
- [x] **T02 [A]** Commit the navbar mark as `public/favicon.svg` and generate the four icons with the command in
  `plan.md`. Done when the PNG files show the mark centred and uncropped.
- [x] **T03 [A]** Add `public/manifest.webmanifest` and the head tags to `index.html`. Done when the new tests
  pass and `npm run build` publishes the manifest and `icons/` at the root of `dist/`.
- [x] **T04 [A]** Add REQ-019 to REQ-021 to `docs/10-requirements.md` and `docs/50-traceability.md`.
- [ ] **T05 [H]** After the merge: the live manifest answers 200, then install on an Android phone and an iPhone.
  Done when the icon, the label `GameHaven` and the window without browser bar are confirmed.
