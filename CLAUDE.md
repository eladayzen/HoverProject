# HoverProject

Endless-runner hover-car prototype built on the **Dreamteck Forever** asset
(`Assets/Dreamteck/Forever/`), controlled through a Unity-MCP bridge
(`mcp__ai-game-developer__*` tools). URP project.

## Active scene

**`Assets/Dreamteck/Forever/Examples/Retro Wave/RetroWaveNarrow.unity`** is the
scene actually being worked on — a duplicate of Dreamteck's original
`OriginalRetroScene.unity` example, narrowed and reskinned. The original
example scene/assets are left untouched; everything project-specific lives in
its own folder (see below) so the vendor example still works as a reference
and can be diffed/reverted against cleanly.

Car: `Assets/Dreamteck/Forever/Examples/Retro Wave/Scripts/HoverPlayer.cs`
(physics hover vehicle) + `HoverPlayerCamera.cs`. `hoverHeight = 0.65`.

## Project-specific assets: `Assets/RetroWaveVariant/`

- `Prefabs/Mountains_{Transit,1,2,3,4}_Narrow.prefab` — independent copies
  (not Prefab Variants — those resist `DestroyImmediate` on inherited
  children) of the original Mountains segment prefabs, narrowed via
  `transform.localScale.x = 0.5`. Ramps kept, Pine trees disabled (not
  deleted). Currently single-submesh, using either the original `Terrain`
  material or `Textures/SewerFloor.mat` — toggle via the Editor menu below.
- `Levels/NarrowLevel.asset` — duplicated `ForeverLevel`, sequences point at
  the `_Narrow` prefabs. Its `RandomByChance` sequence's `randomizer` field
  must reference the shared `UnityRandomizer` asset (originally
  instanceID 54884) — leaving it null causes a `NullReferenceException` in
  `SegmentSequence.GetRandomSegmentByChance`.
- `Levels/RandomPathGenerator_Narrow.asset` — duplicate of the original
  `RandomPathGenerator.asset`, same randomizer/pitch config, but with
  `minSegmentOffset`/`maxSegmentOffset`/`segmentOffsetSpace` configured (see
  "Segment gap" below). `RetroWaveNarrow`'s `LevelGenerator._sharedPathGenerator`
  points here instead of the original shared asset.
- `Textures/` — `SewerFloor.mat`/`SewerWall.mat` (URP Lit, floor/wall diffuse
  + normal maps, currently tiled at `(0.5, 1.5)` scale — this is the
  user-approved "x10" size after iterating down from `(5,15)`).
- `Sky/NebulaSkybox.mat` — `Skybox/Panoramic` shader over a user-generated
  nebula image, resized from 21:9 to the required 2:1 equirectangular ratio
  (4096×2048). Assigned as `RetroWaveNarrow`'s `RenderSettings.skybox`.
- `Editor/TerrainTextureSwitcher.cs` — **Tools → Retro Wave → Terrain
  Texture → Original / Sewer Floor** menu, one-click swap of the terrain
  material across all 5 `_Narrow` prefabs. Disabled during Play Mode.
- `KillZone.cs` — large trigger well below the map (Y=-20, size
  2000×10×2000) that calls `Dreamteck.Forever.EndScreen.Open()` on contact,
  same crash/restart flow as normal crashes. Positioned low enough to clear
  the terrain's lowest dip (~Y=-11.5) — if repositioning it, check the
  terrain's actual bounds first, a prior version overlapped legitimate low
  ground and killed the player incorrectly.

## Segment gap (added 2026-07, commit `3d1a43c`)

Consecutive segments used to be perfectly stitched (zero gap — `LevelSegment
.stitch`, `LevelSegmentExtrusion.cs:79-83`, forces the new segment's first
path sample to exactly match the previous segment's last one). Combined with
the level's pitch variation (`usePitch`, up to 45°, from the original
Dreamteck example config — not something built for this project), this
caused a "touching two segments at once" collision glitch at segment
boundaries that could break the game (likely tripping `HoverPlayer
.OnCollisionStay`'s upside-down check via a bad contact normal at the
crease).

Fix uses `RandomPathGenerator`'s existing (vendor, not custom-built)
`minSegmentOffset`/`maxSegmentOffset` + `OffsetPoints()` mechanism — offsets
every point of a newly-generated segment by a small vector in `Space.Self`
(relative to the *previous* segment's exit direction, so it's correct
regardless of the path's current yaw/pitch), and **automatically disables
`stitch`** per-segment whenever the offset is non-zero. No custom extrusion
or threading code was needed — see `RandomPathGenerator.cs:240-255`.

Current values (`RandomPathGenerator_Narrow.asset`): gap ~1.5-2.5 units
forward, ~1-2 units downward drop, per segment transition, `Space.Self`.
**This is cumulative** — the level loops the same 5 segments forever
(`LevelGenerator._levelIteration = OrderedLoop`, `_loopSegmentLogic = true`),
so the track keeps sinking indefinitely over a long session (unlike the
pitch descent, which is self-clamped by `restrictPitch`). The kill-zone at
Y=-20 is the safety net for now; a hard cap/reset wasn't requested yet.

## Abandoned experiments (left in place, not cleaned up)

- `Assets/TurtleTunnel/` — an earlier attempt at a fully custom procedural
  tunnel mesh (narrow tall half-cone shape). Rejected by design review
  ("it's not good"). Left in place per explicit instruction; not referenced
  by the active scene.
- Wall-raising + floor/wall submesh split on the terrain meshes (giant-sewer
  side walls) — built once, reverted (commit `b46bcc2`) after a mesh rebuild
  performed *while the user was in Play Mode* corrupted a segment mid-extrude
  and crashed `LevelSegment.ExtrudeObject`/`TS_Mesh.CreateFromMesh` on
  Dreamteck's background extrusion thread. The floor/wall textures survived
  the revert and were reused for the floor-only reskin. **Standing rule
  since this incident: never edit a Mesh/prefab/asset that the active scene
  references while Play Mode is running — check `Application.isPlaying`
  first, ask if unsure.**

## Other top-level project folders

- `Assets/BallRoller/` — separate, self-contained "auto-roll lane switcher"
  ball prototype, unrelated to the hover-car track.
- `Assets/Characters/Skater/` — rigged skateboarding character attached
  under the car in the Retro Wave scene, with an occasional "trick"
  animation state reachable from its idle loop (root-motion drift on the
  trick clip was manually detrended via curve editing, not importer flags).
  Exported as a standalone `.unitypackage` to
  `~/Documents/3d/Exports/SkaterCharacter.unitypackage` for portability.

## Working conventions

- **Duplicate Dreamteck's original assets, never mutate them in place.**
  Everything project-specific lives in `Assets/RetroWaveVariant/` (or its own
  self-contained folder, e.g. `Assets/Characters/Skater/`), referencing but
  not editing the vendor originals under `Assets/Dreamteck/`.
- **Never edit an asset that the currently-open scene references while Play
  Mode is active.** Check `Application.isPlaying` before any script-driven
  asset/prefab edit; ask first if the user might be testing.
- **Never `git push` (or commit) without the user asking in that specific
  turn.** A prior "push" request doesn't carry forward to later changes in
  the same session.
- `.mcp.json` and `.claude/settings.local.json` are gitignored — they hold a
  live MCP bearer token. Never commit them; this repo is public.
