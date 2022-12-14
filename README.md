# LIMS

A full-stack C# WPF project, parsing raw output from LC-MS/MS instrument control software and performing regression analysis. This includes the ability to save and load data within a project structure, manually update regression parameters and exclude data points, and view the calibration curve graphically

The project is designed using MVVM architecture to allow for testability and maintainability. 

Sample exports for import are available in /SampleExports.

## Project Creation
Users can create a project structure using dialogs, with analytical run data stored on the local disk.
![LIMSProjects](https://user-images.githubusercontent.com/22641037/205672773-d340dd9e-16ef-4666-9db7-a8f8721c2e1f.jpg)

## Viewing the Regression Data
Data is imported from a .txt export file generated by the instrument control software, which is then parsed. A regression is then run and the results displayed in a grid. The user can deactivate calibration standard and QC samples to exclude them from the analysis.
![LIMSDataRows](https://user-images.githubusercontent.com/22641037/205673013-a11eadfb-2c57-4ec2-aa60-65756d3fc189.jpg)

## Calibration Curve
The calibration curve is rendered using oxyplot, allowing zoom and pan across the graph.
![LIMSInstrumentResponse](https://user-images.githubusercontent.com/22641037/205673019-14c945b6-f57b-43f4-beb4-45077005c7f9.jpg)
