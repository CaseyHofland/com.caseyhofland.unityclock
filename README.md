# UnityClock
UnityClock is a time-of-day system for Unity that allows full creative flexibility with a non-destructive workflow.

## Animation Clips
The main concept behind UnityClock is its utilization of the Animation System. UnityClock isn't like most time systems that provide components for things like rotating the sun, changing the color of the light, or any other hard-coded solution to your time-of-day needs. Instead, simply add a TemporalAnimation component to anything you want to animate with time and provide it with an animation clip. By using animation clips, it gives you full creative flexibility of when you want to change what and how you want to do it.

## Version Control
But the most important part of using animations is version control. Changing time will not mark your scene dirty in the editor, leaving you free to create and experiment without the fear of breaking someone elses project.

## Previewing
You can also preview time and time lapses without entering playmode! In the scene view, open the Overlay Menu and expose the Clock Tools. There you will find the tools required to preview how your scene will look during any time of day.

## Clock Volume
To change time in your scene, you can either set it using the Clock.time variable, or the much preferred method: using the Clock Volume. Unity's volume component allows you to blend between post processing, but this asset repurposes it as a means to blend between time. The great benefit of time blending is so that you can specify parts in your game where the time HAS to be 22:35, so you just put a volume there with some blend distance and everything will work as expected.

## Shader Variables
If you want to create shaders that listen to the time of day, there are 3 values exposed: _UnityClock_Time, _UnityClock_Interpolant and _UnityClock_PingPong. _UnityClock_Time is an int4 which holds time in a 23:59:59.999 format and will in most cases be useless to you. _UnityClock_Interpolant is a value from 0 to 1 between midnight and midnight. _UnityClock_PingPong is a value from 0 to 1 between midnight and midday. These 2 are on average much more useful.

## API Ease of Use
The API uses System.TimeSpan and System.TimeOnly for maximum comfort around coding. Any API that uses float for specifying time can take a hike! `Clock` holds the current time of day, as well as some handy time-related methods. If you want to draw the inspector for Timespan and TimeOnly, use `[TimeSpan] public long time;` and `[TimeOnly] public long timeSpan;` respectively.
