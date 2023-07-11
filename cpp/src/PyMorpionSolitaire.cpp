#include <pybind11/pybind11.h>
#include "../include/GraphGame.h"

namespace py = pybind11;

PYBIND11_MODULE(PyMorpionSolitaire, m)
{
    m.doc() = "Morpion Solitaire module. Author: Marc Gillioz. Date: July 2023";

    py::class_<GraphGame>(m, "Game")
            .def(py::init<>())
            .def("playByIndex", py::overload_cast<int>(&GraphGame::play))
            .def("playAtRandom", py::overload_cast<>(&GraphGame::playAtRandom))
            .def("playAtRandom", py::overload_cast<int>(&GraphGame::playAtRandom))
            .def("playNestedMC", &GraphGame::playNestedMC)
            .def("undo", py::overload_cast<>(&GraphGame::undo))
            .def("undo", py::overload_cast<int>(&GraphGame::undo))
            .def("restart", &GraphGame::restart)
            .def("revertToScore", &GraphGame::revertToScore)
            .def("revertToRandomScore", &GraphGame::revertToRandomScore)
            .def("getScore", &GraphGame::getScore)
            .def("getNumberOfMoves", &GraphGame::getNumberOfMoves)
            .def("print", &GraphGame::print);
}