#include "../include/Image.h"
#include "../include/Coordinates.h"

#include <algorithm>
#include <cstring>
#include <iostream>
#include <string>

using std::optional;

void Image::clear()
{
    std::memset(value, false, sizeof(value));
}

optional<Move> Image::tryBuildMove(const Line& line, int length, bool disjoint) const
{
    int diff = line.pt2 - line.pt1;
    int directions[4] = {length * HORIZONTAL, length * VERTICAL, length*DIAGONAL1, length*DIAGONAL2};

    if (diff != directions[0] && diff != directions[1] && diff != directions[2] && diff != directions[3])
        return {}; // the point do not have the correct separation to define a segment

    int d = diff / (3 * length);
    int iMin = disjoint ? -1 : 0;
    int iMax = disjoint ? 3 * length + 1 : 3 * length;

    vector<Point> points;
    vector<Point> existingDots;
    optional<Point> dot;

    for (int i = iMin; i <= iMax; i++){
        Point pt = line.pt1 + i * d;
        if (i % 3 == 0) // dot element
        {
            if (value[pt])
                existingDots.emplace_back(pt);
            else
            {
                if (dot.has_value())
                    return {}; // there cannot be more than one new dot
                dot.emplace(pt);
                points.emplace_back(pt);
            }
        }
        else // line element
        {
            if (value[pt])
                return {}; // all line elements must be free
            points.emplace_back(pt);
        }
    }

    if (!dot.has_value())
        return {}; // there must be one new dot

    return Move(dot.value(), line, points, existingDots);
}

bool Image::isValidMove(const ImageMove& move) const
{
    return all_of(move.begin(), move.end(), [this](Point pt) { return !value[pt]; });
}

bool Image::isValidMove(const Move& move) const
{
    return all_of(move.begin(), move.end(), [this](Point pt) { return !value[pt]; })
        && all_of(move.existingDots.begin(), move.existingDots.end(), [this](Point pt) { return value[pt]; });
}

void Image::apply(const ImageMove& move, bool value)
{
    for (Point pt: move)
        this->value[pt] = value;
}

void Image::print(int xMin, int xMax, int yMin, int yMax) const
{
    const char symbols[] = {'O', '-', '/', '|', '\\'};

    std::string output = std::string(xMax - xMin + 3, 'X') + '\n';
    for (int y = yMin; y <= yMax; y++)
    {
        output += "X";
        for (int x = xMin; x <= xMax; x++)
        {
            if (value[makePoint(x, y)])
                output += symbols[std::abs(x % 3 + 3 * (y % 3) - 4)];
            else
                output += ' ';
        }
        output += "X\n";
    }
    output += std::string(xMax - xMin + 3, 'X');

    std::cout << std::endl << output << std::endl << std::endl;
}