# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- A skinnable bar during gameplay to represent the time left in the map.
- Hitsounds and misssounds. Control effect volume by changing `EffectVolume` in config.ini.
- MissSoundThreshold to config.ini. This setting determines how many notes will pass before the misssound will play again.
- Option to disable hitsounds for specific judgements in skin/audio.ini

### Changed
- Scrolling in the song select or settings menu is no longer infinite.

### Removed
- The `AllMessages` setting in config.ini.

### Fixed
- Major framerate stutter issues for some users.
- A game crash when pressing backspace when there's no text in the box.
- Lag issues when both Vsync and FPSLimit are utilized
- Inconsistent typing in the searchbar of the Song Select.
- Process not quitting when clicking the X button or Alt+F4'ing
- Screenshot tool not working and/or crashing the game (hopefully)

## [1.3.2-alpha] - 2020-01-27

### Changed
- Backspace gets rid of the last character instead of clearing the searchbox.
- Pressing escape clears the searchbox if there is text in it. When there's no text, pressing escape quits to the main menu.

### Fixed
- Searchbox entering the names of keys instead of proper typing (i.e. "space", "leftalt")
- Positioning of the elements on ScoreCards and the Grade on the result screen.
- Leaderboards only showing at max the same amount of ranks as there were beatmap cards on the screen.
- Sliders not dragging.

## [1.3.1-alpha] - 2020-01-26

### Fixed
- Crosshair fading when hidden is not enabled.

## [1.3.0-alpha] - 2020-01-26

### Added
- **Experimental**: Crosshair turns invisible when playing with Hidden.
- Option to batch convert multiple maps in a single folder.
- Optimization to Song Select.
- Delay to refreshing the Song Select after typing in the search bar.
- Screenshot function, screenshot key defined in `config.ini` (default for now is F11), images save to Pulsarc/Screenshots 

### Fixed
- Setting decimal numbers for ApproachRate crashing the game.
- Unresponsiveness from some buttons.
- Inconsistent Beatmap card movement in the Song Select.
- Pressing Delete/Backspace causing the Song Select to refresh.

### Changed
- Drawables now scale their positioning on both axis, instead of just one. This makes working with skini's more consistent and have less guess-work.

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

### Changed
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


[unreleased]: https://github.com/PulsarcGame/Pulsarc/compare/v1.3.2-alpha...HEAD
[1.3.2-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.3.1-alpha...v1.3.2-alpha
[1.3.1-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.3.0-alpha...v1.3.1-alpha
[1.3.0-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.2.0-alpha...v1.3.0-alpha
[1.2.0-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.1.1-alpha...v1.2.0-alpha
[1.1.1-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.1.0-alpha...v1.1.1-alpha
[1.1.0-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.0.1-alpha...v1.1.0-alpha
[1.0.1-alpha]: https://github.com/PulsarcGame/Pulsarc/compare/v1.0.0-pre-release...v1.0.1-alpha
[1.0.0-pre-release]: https://github.com/PulsarcGame/Pulsarc/tree/v1.0.0-pre-release
