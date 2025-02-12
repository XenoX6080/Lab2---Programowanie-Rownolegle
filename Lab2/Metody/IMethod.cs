﻿internal interface IMethod
{
    string Name { get; }
    double Integrate(IFunction function, double start, double end, int steps, IProgress<int> progress, CancellationToken token);

}

