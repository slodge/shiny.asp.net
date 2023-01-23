# Shiny in .net

Demo: [https://shinyaspnet.azurewebsites.net/](https://shinyaspnet.azurewebsites.net/)

This repo is an experimental implementation of some [Shiny](https://shiny.rstudio.com/) features in .Net 

The project builds upon a standard RazorPages Asp.Net project. It includes the pre-built [shiny.js](https://github.com/rstudio/shiny/blob/main/inst/www/shared/shiny.js) javascript file inside the client pages, then adds a small Websocket layer, coupled with [Rx.Net](https://github.com/dotnet/reactive) to provide some server-side functionality.

Three example "Shiny pages" are included:

- a simple button press input, with a text output displaying the button press count.
- a numeric slider with a plot output (using plotting from the fabulous [ScottPlot.NET](https://scottplot.net/) library.
- a date range picker with a plot output (again using [ScottPlot.NET](https://scottplot.net/)) 

# More details to follow...

The current status is very rough (the `async` and the `Reactive` scheduling are not particularly robust...)

