# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.9.0-pre] - 2023-08-21
### Added
- Add `TimeOnlyField` and `TimeSpanField`, proper implementations for `TimeOnly` and `TimeSpan` in the inspector. These fields can be dragged while still allowing for string inputs.
- Add `Clock.Day()` to return a normalized `float` corresponding to the time of day. e.g. `12:00 = 0.5`, `18:00 = 0.75`, etc.
- Add `ClockInstance` as a component that can easily be added to a scene to set your `Clock` in a simple, lightweight manner.
- Add `Clock.playableGraph` in order to connect animations, sounds and functionality to the `Clock` via `Playables`. This is much more scaleable and memory efficient.
- Add `CycleAnimator` to replace `TemporalAnimation`. This connects to `Clock.playableGraph` in order to be more scaleable and memory efficient.
- Add `ICycle` and `CyclePlayableExtensions` in order to set up `Playables` as `Cycles`. It is pretty hacky, however the `Playables` API is sealed tight, so this was a good compromise.
- Add `AnimationClipCycle` in order to set up a `AnimationClipPlayable` as an `AnimationClipCycle`.

### Changed
- `TimeOnlyAttribute` and `TimeSpanAttribute` no longer take additional parameters as the improved inspector implementation doesn't need them.
- `_UnityClock_Interpolant` has been renamed to `_UnityClock_Day`.
- `_UnityClock_PingPong` has been renamed to `_UnityClock_Midday`.
- `Clock.PingPong()` has been renamed to `Clock.Midday()`.
- Changed the Clock Tools to use one window combining the new `TimeOnlyField` and `TimeSpanField`.

### Removed
- `TemporalAnimation` in favor of `CycleAnimator`.
- Experimental `TimeRange`, `ITemporal` & `TemporalAnimationProcessor`.
- Experimental `ClockVolume`. A different solution is required in order to blend time based on position.

## [0.1.0-exp] - 2023-05-15
### This is the first release of *Unity Clock*.
Unity Clock is a time-of-day system for Unity that allows full creative flexibility with a non-destructive workflow.