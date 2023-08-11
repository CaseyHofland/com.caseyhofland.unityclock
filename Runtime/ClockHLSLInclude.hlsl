//UNITY_SHADER_NO_UPGRADE
#ifndef CLOCKHLSLINCLUDE_INCLUDED
#define CLOCKHLSLINCLUDE_INCLUDED
int4 _UnityClock_Time;
float _UnityClock_Day;
float _UnityClock_Midday;

void ClockValues_float(out int4 time, out float day, out float midday)
{
	time = _UnityClock_Time;
	day = _UnityClock_Day;
	midday = _UnityClock_Midday;
}

void ClockValues_half(out int4 time, out half day, out half midday)
{
	time = _UnityClock_Time;
	day = _UnityClock_Day;
	midday = _UnityClock_Midday;
}
#endif //CLOCKHLSLINCLUDE_INCLUDED