#include "../include/Image.h"
#include "../include/Coordinates.h"

#include <algorithm>
#include <cstring>
#include <iostream>
#include <string>

using std::all_of, std::optional;

template <size_t length, bool disjoint>
void Image<length, disjoint>::clear()
{
    std::memset(value, false, sizeof(value));
}

template <size_t length, bool disjoint>
optional<Move<length, disjoint>> Image<length, disjoint>::tryBuildMove(const Line& line) const
{
    int diff = line.pt2 - line.pt1;
    int directions[4] = {(int)length * HORIZONTAL, (int)length * VERTICAL, (int)length*DIAGONAL1, (int)length*DIAGONAL2};

    if (diff != directions[0] && diff != directions[1] && diff != directions[2] && diff != directions[3])
        return {}; // the point do not have the correct separation to define a segment

    int d = diff / (3 * length);

    ImageMove<length, disjoint> points;
    array<Point, length> existingDots;
    optional<Point> dot;

    int iMin = disjoint ? -1 : 0;
    int iMax = disjoint ? 3 * length + 1 : 3 * length;
    auto pointsIterator = points.begin();
    auto existingDotsIterator = existingDots.begin();

    for (int i = iMin; i <= iMax; i++)
    {
        Point pt = line.pt1 + i * d;
        if (i % 3 == 0) // dot element
        {
            if (value[pt])
            {
                *existingDotsIterator = pt;
                existingDotsIterator++;
            }
            else
            {
                if (dot.has_value())
                    return {}; // there cannot be more than one new dot
                dot.emplace(pt);
                *pointsIterator = pt;
                pointsIterator++;
            }
        }
        else // line element
        {
            if (value[pt])
                return {}; // all line elements must be free
            *pointsIterator = pt;
            pointsIterator++;
        }
    }

    if (!dot.has_value())
        return {}; // there must be one new dot

    return Move<length, disjoint>(*dot, line, points, existingDots);
}

template <size_t length, bool disjoint>
bool Image<length, disjoint>::isValidMove(const ImageMove<length, disjoint>& move) const
{
    return all_of(move.begin(), move.end(), [this](Point pt) { return !value[pt]; });
}

template <size_t length, bool disjoint>
bool Image<length, disjoint>::isValidMove(const Move<length, disjoint>& move) const
{
    return all_of(move.begin(), move.end(), [this](Point pt) { return !value[pt]; })
        && all_of(move.existingDots.begin(), move.existingDots.end(), [this](Point pt) { return value[pt]; });
}

template <size_t length, bool disjoint>
void Image<length, disjoint>::apply(const ImageMove<length, disjoint>& move, bool value)
{
    for (Point pt: move)
        this->value[pt] = value;
}

template <size_t length, bool disjoint>
void Image<length, disjoint>::print(int xMin, int xMax, int yMin, int yMax) const
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

template class Image <4, false>;
template class Image <4, true>;
template class Image <3, false>;
template class Image <3, true>;