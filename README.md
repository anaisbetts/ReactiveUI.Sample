## ReactiveUI.Sample 3.0

This sample app demonstrates the Best Practices™ way to write an application
using ReactiveUI 3.0. It also tries to show off as many features as possible
of RxUI, while teaching the Zen of the framework.

The application itself is a WPF 4.0 application to display your Issues on
GitHub, modeled after the GH Issues app for iPhone.

## Annoying Note

At the moment, you must have .NET 4.5 Beta installed to run the code, and you
must apply a binding redirect to your test runner for JSON.NET in order to run
the tests. Both of these issues should be resolved in the near future, so this
should be as simple as checking out and building any other app.

This sample is also currently using a hacked version of RxUI 3.0 for test
purposes, but it will soon use RxUI 3.0.5 which will soon be on NuGet once the
Rx issues around .NET 4.5 are resolved.