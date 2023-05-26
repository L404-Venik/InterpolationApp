// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include "mkl.h"
#include <iostream>
#include <chrono>
#include <fstream>

using namespace std;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" _declspec(dllexport)

void Call_MKL(
    const double* Nodes,
    const MKL_INT RawDataNNodes,
    const bool Uniform,
    const double* Values,
    const double* Derivatives,
    const MKL_INT SplineDataNNodes,
    const double* Ends,
    double* SplineParameters,
    double* Integral,
    int* ErrorType)
{
    DFTaskPtr task;

    if (dfdNewTask1D(
        &task,          // дескриптор задачи
        RawDataNNodes,  // число узлов сетки
        Nodes,          // массив узлов интерполяции 
        Uniform ? DF_UNIFORM_PARTITION : DF_NON_UNIFORM_PARTITION,    // тип разбиения отрезка
        1,              // размерность векторной функции 
        Values,         // массив значений  функции 
        DF_NO_HINT)     // без дополнительной информации
        != DF_STATUS_OK)
    { 
        *ErrorType = 1;
        return;
    }

    double* scoeff =  new double[DF_PP_CUBIC * (RawDataNNodes - 1)];
    if (dfdEditPPSpline1D(
        task,               // дескриптор задачи
        DF_PP_CUBIC,        // порядок сплайна
        DF_PP_NATURAL,      // тип сплайна
        DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER, //  тип граничных условий 
        Derivatives,        // значения производной на концах отрезка
        DF_NO_IC,           // тип условий во внутренних точках
        nullptr,                  // условия во внутренних точках (без условий)
        scoeff,   // массив коэффициентов сплайна
        DF_NO_HINT)         // без дополнительной информации
        != DF_STATUS_OK)
    {
        *ErrorType = 2;
        return;
    }
    
    if (dfdConstruct1D(
        task, 
        DF_PP_SPLINE, 
        DF_METHOD_STD
    ) != DF_STATUS_OK)
    {
        *ErrorType = 3;
        return;
    }

    MKL_INT dorder[3]{ 1 , 1 , 1 };  // вычисляются значения функции, первой и второй производных
    if (dfdInterpolate1D(
        task,
        DF_INTERP,
        DF_METHOD_PP,
        SplineDataNNodes,
        Ends, 
        DF_UNIFORM_PARTITION,//DF_SORTED_DATA,
        3,  
        dorder,
        NULL,
        SplineParameters,
        DF_MATRIX_STORAGE_ROWS,
        NULL
    ) != DF_STATUS_OK)
    {
        *ErrorType = 4;
        return;
    }

    const MKL_INT nlim = 1;
    if (dfdIntegrate1D(
        task,
        DF_METHOD_PP,
        nlim,
        &Ends[0],
        DF_NO_HINT,
        &Ends[1],
        DF_NO_HINT,
        nullptr,
        nullptr,
        Integral,
        DF_NO_HINT
    ) != DF_STATUS_OK) {
        *ErrorType = 5;
        return;
    }
    delete[] scoeff;
    dfDeleteTask(&task);
}