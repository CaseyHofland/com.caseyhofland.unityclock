//UNITY_SHADER_NO_UPGRADE
#ifndef CLOCKHLSLINCLUDE_INCLUDED
#define CLOCKHLSLINCLUDE_INCLUDED
int4 _UnityClock_Clock_Time;
float _UnityClock_Clock_Interpolant;
float _UnityClock_Clock_PingPong;

void ClockValues_float(out int4 time, out float interpolant, out float pingPong)
{
	time = _UnityClock_Clock_Time;
	interpolant = _UnityClock_Clock_Interpolant;
	pingPong = _UnityClock_Clock_PingPong;
}

void ClockValues_half(out int4 time, out half interpolant, out half pingPong)
{
	time = _UnityClock_Clock_Time;
	interpolant = _UnityClock_Clock_Interpolant;
	pingPong = _UnityClock_Clock_PingPong;
}
#endif //CLOCKHLSLINCLUDE_INCLUDED