#include "../include/GridDTO.h"
#include "../include/Move.h"
#include <json/json.h>

const string GridDTO::TITLE = "Morpion Solitaire";
const string GridDTO::VERSION = "1.0";

GridDTO::GridDTO(const Grid &grid)
    : length(grid.length), disjoint(grid.disjoint)
{
    for (Point pt: grid.initialDots)
        initialDots.push_back({getX(pt) / 3, getY(pt) / 3});

    for (const GridMove& move: grid.moves)
        moves.push_back({getX(move.line.pt1) / 3, getY(move.line.pt1) / 3,
                         getX(move.line.pt2) / 3, getY(move.line.pt2) / 3,
                         getX(move.dot) / 3, getY(move.dot) / 3});
}

GridDTO::GridDTO(const string &json)
{
    Json::Value root;
    Json::Reader reader;
    reader.parse(json, root);

    if (root["Title"].asString() != TITLE || root["Version"].asString() != VERSION)
        throw std::logic_error("Incompatible file format");

    length = root["LineLength"].asInt();
    disjoint = root["Disjoint"].asBool();

    for (auto value: root["InitialDots"])
    {
        array<int, 2> dot = {0, 0};
        for (int i = 0; i < 2; i++){
            dot[i] = value[i].asInt();
        }
        initialDots.emplace_back(dot);
    }

    for (auto value: root["Moves"])
    {
        array<int, 6> move = {0, 0, 0, 0, 0, 0};
        for (int i = 0; i < 6; i++){
            move[i] = value[i].asInt();
        }
        moves.emplace_back(move);
    }
}

Grid GridDTO::toGrid() const
{
    vector<Point> gridInitialDots;
    for (array<int, 2> pt: initialDots)
        gridInitialDots.emplace_back(makePoint(3 * pt[0] + 1, 3 * pt[1] + 1));

    vector<GridMove> gridMoves;
    for (array<int, 6> move: moves)
        gridMoves.emplace_back(makePoint(3 * move[4] + 1, 3 * move[5] + 1),
                               Line(makePoint(3 * move[0] + 1, 3 * move[1] + 1),
                                    makePoint(3 * move[2] + 1, 3 * move[3] + 1)));

    return {length, disjoint, gridInitialDots, gridMoves};
}

string GridDTO::toJSON() const
{
    Json::Value root;
    root["Title"] = TITLE;
    root["Version"] = VERSION;
    root["LineLength"] = length;
    root["Disjoint"] = disjoint;

    Json::Value jsonInitialDots(Json::arrayValue);
    for (array<int, 2> dot: initialDots)
    {
        Json::Value coordinates(Json::arrayValue);
        for (int coord: dot)
            coordinates.append(coord);
        jsonInitialDots.append(coordinates);
    }
    root["InitialDots"] = jsonInitialDots;

    Json::Value jsonMoves(Json::arrayValue);
    for (array<int, 6> move: moves)
    {
        Json::Value coordinates(Json::arrayValue);
        for (int coord: move)
            coordinates.append(coord);
        jsonMoves.append(coordinates);
    }
    root["Moves"] = jsonMoves;

    Json::FastWriter writer;
    return writer.write(root);
}