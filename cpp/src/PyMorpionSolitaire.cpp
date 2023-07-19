#include <pybind11/pybind11.h>
#include <pybind11/embed.h>
#include <sstream>
#include "../include/GraphGame.h"
#include "../include/GridFootprint.h"

using namespace std;
namespace py = pybind11;

class PyGraphGame : public GraphGame
{
public:
    using GraphGame::GraphGame; // inherit constructor

    void print() const override
    {
        GridFootprint footprint(grid);
        footprint.pad(3);
        int offset = min(footprint.min.x(), footprint.min.y());
        int size = max(footprint.max.x(), footprint.max.y()) - offset + 1;

        stringstream printCommand;
        printCommand << "import matplotlib.pyplot as plt" << endl;
        printCommand << "size = " << size << endl;
        printCommand << "offset = " << offset << endl;
        printCommand << "viewwindow = (offset, offset + size - 1)" << endl;
        printCommand << "viewrange = range(offset, offset + size)" << endl;
        printCommand << "fig = plt.figure(figsize=(5,5))" << endl;
        printCommand << "fig.add_subplot(aspect=1, autoscale_on=False, xlim=viewwindow, ylim=viewwindow, "
            << "xticks=viewrange, xticklabels='', yticks=viewrange, yticklabels='')" << endl;
        printCommand << "plt.grid()" << endl;

        for (Point pt: grid.initialDots)
        {
            Coordinates p(pt);
            printCommand << "plt.plot(" << p.x() << "," << p.y() << ", color='r', marker='o', markersize=5)" << endl;
        }

        for (GridMove move: grid.moves)
        {
            Coordinates p1(move.line.pt1);
            Coordinates p2(move.line.pt2);
            printCommand << "plt.plot([" << p1.x() << "," << p2.x() << "], ["
                << p1.y() << ", " << p2.y() << "], color='k')" << endl;
            Coordinates p(move.dot);
            printCommand << "plt.plot(" << p.x() << "," << p.y()
                << ", color='k', marker='o', markersize=4)" << endl;
        }

        printCommand << "plt.title('Score: " << GraphGame::getScore()
            << "    Number of possible moves: " << GraphGame::getNumberOfMoves() << "')" << endl;

        py::object scope = py::module_::import("__main__").attr("__dict__");
        py::exec(printCommand.str(), scope);
    }
};

PYBIND11_MODULE(PyMorpionSolitaire, m)
{
    m.doc() = "Morpion Solitaire module. Author: Marc Gillioz. Date: July 2023";

    py::class_<PyGraphGame>(m, "Game")
            .def(py::init<char, int, bool>(),
                    py::arg("type") = 'c', py::arg("length") = 4, py::arg("disjoint") = false)
            .def("playByIndex", py::overload_cast<int>(&PyGraphGame::play))
            .def("playAtRandom", py::overload_cast<>(&PyGraphGame::playAtRandom))
            .def("playAtRandom", py::overload_cast<int>(&PyGraphGame::playAtRandom))
            .def("playNestedMC", py::overload_cast<int>(&PyGraphGame::playNestedMC))
            .def("undo", py::overload_cast<>(&PyGraphGame::undo))
            .def("undo", py::overload_cast<int>(&PyGraphGame::undo))
            .def("restart", &PyGraphGame::restart)
            .def("revertToScore", &PyGraphGame::revertToScore)
            .def("revertToRandomScore", &PyGraphGame::revertToRandomScore)
            .def("getScore", &PyGraphGame::getScore)
            .def("getNumberOfMoves", &PyGraphGame::getNumberOfMoves)
            .def("print", &PyGraphGame::print);
}