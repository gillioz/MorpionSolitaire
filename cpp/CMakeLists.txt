cmake_minimum_required(VERSION 3.25)
project(CppMorpionSolitaire)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_FLAGS "-O3 -fPIC")

find_package(pybind11 REQUIRED)
find_package(jsoncpp REQUIRED)

set(GAME_SOURCES
        src/Game.cpp
        src/Grid.cpp
        src/Image.cpp
        src/Line.cpp
        src/Coordinates.cpp
        src/Point.cpp
        src/GridFootprint.cpp
        src/GridDTO.cpp)

set(GRAPHGAME_SOURCES
        src/GraphGame.cpp)

add_executable(GameTests tests/GameTest.cpp ${GAME_SOURCES})
target_include_directories(GameTests PUBLIC include)
target_link_libraries(GameTests PUBLIC jsoncpp_lib)

add_executable(GraphGameTests tests/GraphGameTest.cpp ${GAME_SOURCES} ${GRAPHGAME_SOURCES})
target_include_directories(GraphGameTests PUBLIC include)
target_link_libraries(GraphGameTests PUBLIC jsoncpp_lib)

add_library(MorpionSolitaire SHARED ${GAME_SOURCES} ${GRAPHGAME_SOURCES})
target_include_directories(MorpionSolitaire PUBLIC include)
target_link_libraries(MorpionSolitaire PUBLIC jsoncpp_lib)

pybind11_add_module(PyMorpionSolitaire src/PyMorpionSolitaire.cpp)
add_dependencies(PyMorpionSolitaire MorpionSolitaire)
target_link_libraries(PyMorpionSolitaire PRIVATE MorpionSolitaire)

add_executable(PlayNestedMC src/PlayNestedMC.cpp)
add_dependencies(PlayNestedMC MorpionSolitaire)
target_link_libraries(PlayNestedMC PRIVATE MorpionSolitaire)

set_target_properties(MorpionSolitaire GameTests GraphGameTests PyMorpionSolitaire PlayNestedMC PROPERTIES
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