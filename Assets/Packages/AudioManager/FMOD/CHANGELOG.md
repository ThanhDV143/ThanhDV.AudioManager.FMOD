# Changelog
All notable changes to this project will be documented in this file.

## [2.0.3] - 2026-01-20
### Fixed
- Fixed UnityEditor in build

## [2.0.2] - 2026-01-20
### Updated
- `PackageImporter`
- `README`

## [2.0.0] - 2026-01-19
### Updated
- Improved the `WaitForInitializeDone()` flow to ensure AudioManager is fully initialized before any `Play`/`Pause`/`Stop` operations.
### Added
- Added a Windows Editor tool to auto-generate code for `FMODBus` and `FMODEventReference`.

## [1.0.2] - 2025-12-26
### Updated
- Update method `PlayLoop()` to easy optimize.
### Added
- `DetachInstanceFromGameObject()` when release `EventInstance`
- Add method `TryGetEventInstance()` to get created `EventInstance`

## [1.0.1] - 2025-09-22
### Fixed
- Volume debug view

## [0.0.2] - 2025-08-17
### Added
- Volume control

## [0.0.1] - 2025-08-17
### Added
- AudioManager