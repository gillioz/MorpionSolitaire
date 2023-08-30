#include <pybind11/pybind11.h>
#include <pybind11/embed.h>
#include <pybind11/numpy.h>
#include <fstream>
#include <sstream>
#include "../include/GraphGame.h"
#include "../include/GridFootprint.h"
#include "../include/GridDTO.h"

using namespace std;
namespace py = pybind11;

template <size_t length, bool disjoint>
class PyGraphGame : public GraphGame<length, disjoint>
{
public:
    using GraphGame<length, disjoint>::GraphGame; // inherit constructor

    void print() const override
    {
        GridFootprint footprint(GraphGame<length, disjoint>::grid);
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

        for (Point pt: GraphGame<length, disjoint>::grid.initialDots)
        {
            Coordinates p(pt);
            printCommand << "plt.plot(" << p.x() << "," << p.y() << ", color='r', marker='o', markersize=5)" << endl;
        }

        for (GridMove move: GraphGame<length, disjoint>::grid.moves)
        {
            Coordinates p1(move.line.pt1);
            Coordinates p2(move.line.pt2);
            printCommand << "plt.plot([" << p1.x() << "," << p2.x() << "], ["
                << p1.y() << ", " << p2.y() << "], color='k')" << endl;
            Coordinates p(move.dot);
            printCommand << "plt.plot(" << p.x() << "," << p.y()
                << ", color='k', marker='o', markersize=4)" << endl;
        }

        printCommand << "plt.title('Score: " << GraphGame<length, disjoint>::getScore()
            << "    Number of possible moves: " << GraphGame<length, disjoint>::getNumberOfMoves() << "')" << endl;

        py::object scope = py::module_::import("__main__").attr("__dict__");
        py::exec(printCommand.str(), scope);
    }

    void save(const string& filename) const
    {
        string json = Game<length, disjoint>::exportJSON();

        ofstream out(filename.c_str());
        out << json;
        out.close();
    }

    py::array_t<bool> getImage() const
    {
        return py::array_t<bool>({IMAGESIZE, IMAGESIZE}, Game<length, disjoint>::image.value);
    }

    py::array_t<bool> getMoveImages() const
    {
        bool currentImage[IMAGESIZE * IMAGESIZE];
        int imageSize = sizeof(currentImage);
        std::copy(Game<length, disjoint>::image.value, Game<length, disjoint>::image.value + imageSize, currentImage);

        const vector<Move<length, disjoint>> & moves = GraphGame<length, disjoint>::nodes.back().branches;
        int n = moves.size();
        bool moveImages[n * IMAGESIZE * IMAGESIZE];
        for (int i = 0; i < n; i++)
        {
            std::copy(currentImage, currentImage + imageSize, moveImages + i * imageSize);
            for (auto pt: moves[i])
                moveImages[i * imageSize + pt] = true;
        }

        return py::array_t<bool>({n, IMAGESIZE, IMAGESIZE}, moveImages);
    }

    static PyGraphGame<length, disjoint> load(const string& filename)
    {
        ostringstream json;

        ifstream in(filename.c_str());
        json << in.rdbuf();
        in.close();

        GridDTO dto(json.str());
        return PyGraphGame<length, disjoint>(dto.toGrid());
    }
};

template <size_t length, bool disjoint>
void declareGame(py::module& m, const string& name)
{
    py::class_<PyGraphGame<length, disjoint>>(m, name.c_str())
            .def(py::init<char>(), py::arg("type") = 'c')
            .def("playByIndex", py::overload_cast<int>(&PyGraphGame<length, disjoint>::play))
            .def("playAtRandom", py::overload_cast<>(&PyGraphGame<length, disjoint>::playAtRandom))
            .def("playAtRandom", py::overload_cast<int>(&PyGraphGame<length, disjoint>::playAtRandom))
            .def("playNestedMC", py::overload_cast<int>(&PyGraphGame<length, disjoint>::playNestedMC))
            .def("playFastNestedMC", py::overload_cast<int>(&PyGraphGame<length, disjoint>::playFastNestedMC))
            .def("undo", py::overload_cast<>(&PyGraphGame<length, disjoint>::undo))
            .def("undo", py::overload_cast<int>(&PyGraphGame<length, disjoint>::undo))
            .def("restart", &PyGraphGame<length, disjoint>::restart)
            .def("revertToScore", &PyGraphGame<length, disjoint>::revertToScore)
            .def("revertToRandomScore", &PyGraphGame<length, disjoint>::revertToRandomScore)
            .def("getScore", &PyGraphGame<length, disjoint>::getScore)
            .def("getNumberOfMoves", &PyGraphGame<length, disjoint>::getNumberOfMoves)
            .def("print", &PyGraphGame<length, disjoint>::print)
            .def("exportJSON", &PyGraphGame<length, disjoint>::exportJSON)
            .def("save", &PyGraphGame<length, disjoint>::save)
            .def("image", &PyGraphGame<length, disjoint>::getImage)
            .def("getMoveImages", &PyGraphGame<length, disjoint>::getMoveImages)
            .def_static("load", &PyGraphGame<length, disjoint>::load);
}

PYBIND11_MODULE(PyMorpionSolitaire, m)
{
    m.doc() = "Morpion Solitaire module. Author: Marc Gillioz. Date: July 2023";

    declareGame<4, false>(m, "Game5T");
    declareGame<4, true>(m, "Game5D");
    declareGame<3, false>(m, "Game4T");
    declareGame<3, true>(m, "Game4D");
}