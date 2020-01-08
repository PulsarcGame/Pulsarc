# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Hidden-mode Crosshair Offset, allows users to manually change the base size of the crosshair for Hidden.
- Option to batch convert multiple maps in a single folder.

### Fixed
- Setting decimal numbers for ApproachRate crashing the game.

## Changed
- Drawables now scale their positioning on both axis, instead of just one. This makes working with skini's more consistent and manageable.

## [1.2.0-alpha] - 2019-12-04

### Added
- Leaderboards update after completing a map.
- Version counter to the main menu
- Borderless Fullscreen option. Makes alt-tabbing from Fullscreen smoother. When `FullScreen = 2` in config.ini or the Resolution is set to 0, this is enabled.
- `AllMessages' to config.ini. Setting this to false makes it so only Error and Warning messages go through.

### Fixed
- Leaderboard card positioning
- Close button or ALT+F4 not closing the game.
- Certain Intralism Map backgrounds crashing the game during conversion.
- Invalid characters crashing the game

## Changed
- How static variables work in Pulsarc.cs, fixes some issues with the game.

## [1.1.1-alpha] - 2019-11-29

### Fixed
- Optimization issues (hopefully) resolved.

## [1.1.0-alpha] - 2019-11-29

### Added
- Display current status through Discord RPC (Playing maps, browsing menus, etc.)

### Changed
- Significantly optimize the Song Select screen.

## [1.0.1-alpha] - 2019-11-22

### Added
- Username option in config.ini
- Song rate and username on leaderboards
- Intralism converter grabs a background from Intralism maps.

### Changed
- Difficulty system adjustment, difficulties should be lower.
- Version suffix is now "alpha" instead of "pre-release"

### Fixed
- Scrolling Direction in the Song-Select menu
- Improved optimization of gameplay.

## [1.0.0-pre-release] - 2019-11-20

### Added
- Core gameplay for 4 keys
- Skini system for skin customization
- Converter for Intralism and osu!mania
- config.ini to track user preferences
- Song select menu
- Settings menu
- Result screen to recap latest play
- Score and replay saving
- Leaderboard for past scores


[unreleased]: https://github.com/PulsarcGame/Pulsarc/compare/v1.2.0-alpha...HEAD
[1.2.0-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.1.1-alpha...v1.2.0-alpha
[1.1.1-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.1.0-alpha...v1.1.1-alpha
[1.1.0-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.0.1-alpha...v1.1.0-alpha
[1.0.1-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.0.0-pre-release...v1.0.1-alpha
[1.0.0-pre-release]: https://github.com/PulsarcGame/Pulsarc/tag/v1.0.0-pre-release
