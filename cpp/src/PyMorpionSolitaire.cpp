#include <pybind11/pybind11.h>
#include <pybind11/embed.h>
#include <sstream>
#include "../include/GraphGame.h"

using namespace std;
namespace py = pybind11;

class PyGraphGame : public GraphGame
{
public:
    using GraphGame::GraphGame; // inherit constructor

    void print() const override
    {
        GridFootprint footprint(grid);
        int padding = 3;
        int offset = min(footprint.xMin, footprint.yMin) - padding;
        int size = max(footprint.xMax, footprint.yMax) - offset + 2 * padding - 2;

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

        for (GridPoint pt: grid.initialDots)
            printCommand << "plt.plot(" << pt.x << "," << pt.y << ", color='k', marker='o', markersize=4)" << endl;

        for (GridMove move: grid.moves)
        {
            printCommand << "plt.plot([" << move.line.pt1.x << "," << move.line.pt2.x << "], ["
                << move.line.pt1.y << ", " << move.line.pt2.y << "], color='k')" << endl;
            printCommand << "plt.plot(" << move.dot.x << "," << move.dot.y
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
            .def("playNestedMC", &PyGraphGame::playNestedMC)
            .def("undo", py::overload_cast<>(&PyGraphGame::undo))
            .def("undo", py::overload_cast<int>(&PyGraphGame::undo))
            .def("restart", &PyGraphGame::restart)
            .def("revertToScore", &PyGraphGame::revertToScore)
            .def("revertToRandomScore", &PyGraphGame::revertToRandomScore)
            .def("getScore", &PyGraphGame::getScore)
            .def("getNumberOfMoves", &PyGraphGame::getNumberOfMoves)
            .def("print", &PyGraphGame::print);
}