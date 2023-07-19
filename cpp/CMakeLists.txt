cmake_minimum_required(VERSION 3.25)
project(CppMorpionSolitaire)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_FLAGS "-O3")

find_package(pybind11 REQUIRED)

set(GAME_SOURCES
        src/Game.cpp
        src/Grid.cpp
        src/Image.cpp
        src/Line.cpp
        src/Coordinates.cpp
        src/Point.cpp
        src/GridFootprint.cpp)

set(GRAPHGAME_SOURCES
        src/GraphGame.cpp)

add_library(MorpionSolitaire SHARED ${GAME_SOURCES} ${GRAPHGAME_SOURCES})
add_executable(GameTests tests/GameTest.cpp ${GAME_SOURCES})
add_executable(GraphGameTests tests/GraphGameTest.cpp ${GAME_SOURCES} ${GRAPHGAME_SOURCES})
pybind11_add_module(PyMorpionSolitaire src/PyMorpionSolitaire.cpp ${GAME_SOURCES} ${GRAPHGAME_SOURCES})

target_include_directories(MorpionSolitaire PUBLIC include)
target_include_directories(GameTests PUBLIC include)
target_include_directories(GraphGameTests PUBLIC include)

set_target_properties(MorpionSolitaire GameTests GraphGameTests PyMorpionSolitaire PROPERTIES
        RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin
        ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin
        LIBRARY_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/bin)

#################################################################################################
###
### IMPORTANT: the path to the PyBind11 and Python libraries must be added as CMAKE options,
###            for instance as follows:
###    -Dpybind11_DIR=/home/marc/anaconda3/envs/MorpionSolitaire/share/cmake/pybind11
###    -DPYTHON_EXECUTABLE=/home/marc/anaconda3/envs/MorpionSolitaire/bin/python3
###
#################################################################################################