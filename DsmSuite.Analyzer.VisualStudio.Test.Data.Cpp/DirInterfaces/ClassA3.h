// The header file ClassA3.h in the DirInterfaces directory represents a clone from the same file ClassA3.h in the DirA directory and must be exactly identical.
// -The version in DirA is included in the visual studio project
// -The version in DirInterfaces is included in the file ClassA2.cpp
// The aim is that the include of the file ClassA3.h in the DirInterfaces is resolved to the original file in DirA.
// This is done to support projects which clone their interfaces to a single path to simplify includes.
#pragma once