//UNITY_SHADER_NO_UPGRADE
#ifndef CLOCKHLSLINCLUDE_INCLUDED
#define CLOCKHLSLINCLUDE_INCLUDED
int4 _UnityClock_Time;
float _UnityClock_Interpolant;
float _UnityClock_PingPong;

void ClockValues_float(out int4 time, out float interpolant, out float pingPong)
{
	time = _UnityClock_Time;
	interpolant = _UnityClock_Interpolant;
	pingPong = _UnityClock_PingPong;
}

void ClockValues_half(out int4 time, out half interpolant, out half pingPong)
{
	time = _UnityClock_Time;
	interpolant = _UnityClock_Interpolant;
	pingPong = _UnityClock_PingPong;
}
#endif //CLOCKHLSLINCLUDE_INCLUDED