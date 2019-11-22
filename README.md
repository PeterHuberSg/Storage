# Storage
C# only library for fast object oriented data storage in RAM and long term storage on local Harddisk. No database required.

For single user applications using a database for permanent data storage is overkill, adds a lot of unnecessary complexity and slows
down the progran execution.

Nowadays PC have lots of RAMS and for many single user application it is possible to keep all the data in RAM and to do queries using Linq, 
which leads to much faster programs than interfacing with a "slow" database. Also the mismatch between data types in DotNet and databases
can be avoided.

To store the data pemanently, it's enough to copy them in local files. This can be done using UTF8 instead of binary, which
makes it easy to inspect and edit these files with any Editor, but the storage requirement is not much bigger. Storing '1234567' as string
takes the same space (7 bytes plus delimiter) like storing it as 8 byte binary.

This library contains high performance Readers and Writers for CSV ('comma' separated values) files and code generator for the 
objetc related data model in RAM, using .Net Core 3.0.
