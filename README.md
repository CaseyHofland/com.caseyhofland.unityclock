# About Unity Clock

![Unity Clock Showcase](https://github.com/CaseyHofland/com.caseyhofland.unityclock/assets/27729987/9d9b4c43-1f8c-4777-a387-d9a0e3a6c42b)

Unity Clock is a time-of-day system that allows full creative flexibility with a non-destructive workflow. The core philosophy is centered around the utilization of [Animation Clips](https://docs.unity3d.com/Manual/AnimationClips.html) to allow full creative flexibility in how an object should change over time. They can also be previewed in the editor without triggering scene changes, which is a huge benefit when you're working with version control systems like [git](https://git-scm.com/) and [plastic](https://www.plasticscm.com/).

## Installing Unity Clock

To install this package, follow the instructions on the [Package Manager documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

## Requirements

This version of Unity Clock is compatible with the following versions of the Unity Editor:

* 2022.2 and later (recommended)

## Known limitations

Unity Clock version 0.1.0-exp includes the following known limitations:

* Depends on the render-pipeline.core for setting time. This dependency will be removed in the future.
* Code is subject to (a lot) of change.
* Performance for a large amount of objects using Temporal Animations is untested.
* Temporal Animations are still very rigid in their use. This workflow needs to be improved.
* Animation events are unlikely to trigger (this will be fixed in the future).
