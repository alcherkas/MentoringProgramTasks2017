// PowerManagement.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

struct ComplexNumber
{
	double real;
	double imaginary;
};


_declspec(dllexport)
double Add(double a, double b)
{
	return a + b;
}


_declspec(dllexport)
ComplexNumber Add(ComplexNumber a, ComplexNumber b)
{
	ComplexNumber result;
	result.real = a.real + b.real;
	result.imaginary = a.imaginary + b.imaginary;

	return result;
}

_declspec(dllexport)
double Summ(double numbers[], int count)
{
	double result = 0;

	for (int i = 0; i < count; i++)
	{
		result += numbers[i];
	}

	return result;
}

_declspec(dllexport)
ComplexNumber Summ(ComplexNumber numbers[], int count)
{
	ComplexNumber result;
	result.real = 0;
	result.imaginary = 0;

	for (int i = 0; i < count; i++)
	{
		result.real += numbers[i].real;
		result.imaginary += numbers[i].imaginary;
	}

	return result;
}

_declspec(dllexport)
POWER_INFORMATION_LEVEL GetLastSleepTime()
{
	return LastSleepTime;
}
